using ClosedXML.Excel;
using Ktm.Racing.Domain.Kompass;
using Ktm.Racing.Domain.MotoDb;

namespace Ktm.Racing.Oracle.Tests;

/// <summary>Stability/automation improvements after the RT22 production run.</summary>
public sealed class StabilityTests
{
    // ------------------------------------------------------------------ run repair (orphaned split leftovers)

    [Fact]
    public void Repair_moves_only_empty_unreferenced_lap_folders()
    {
        using var tmp = new TempDir();
        var run = CreateRun(tmp, "Run_1065",
            data: ["Lap_1_Def_000", "Lap_1_Def_001"],
            empty: ["Lap_1", "Lap_1_Def_002"],
            referenced: ["Lap_1_Def_000", "Lap_1_Def_001", "Lap_1_Def_002"]);

        Assert.Equal([Path.Combine(run, "Lap_1")], RunRepair.FindOrphanLaps(run)); // Def_002 is referenced → kept

        var result = RunRepair.Repair(run);

        Assert.Equal(["Lap_1"], result!.MovedLaps);
        Assert.True(Directory.Exists(Path.Combine(run, RunRepair.RemovedFolderName, "Lap_1")));
        Assert.True(File.Exists(Path.Combine(run, RunRepair.RemovedFolderName, "README.txt")));
        Assert.True(Directory.Exists(Path.Combine(run, "Lap_1_Def_002")));
        Assert.Null(RunRepair.Repair(run)); // idempotent
        Assert.Empty(RunFolderInspector.FindRunFolders(Path.Combine(run, RunRepair.RemovedFolderName)));
    }

    [Fact]
    public void Repair_does_nothing_without_referenced_data()
    {
        using var tmp = new TempDir();
        var noHeader = CreateRun(tmp, "Run_1", data: [], empty: ["Lap_1"], referenced: null);
        var allEmpty = CreateRun(tmp, "Run_2", data: [], empty: ["Lap_1", "Lap_2"], referenced: ["Lap_1"]);

        Assert.Empty(RunRepair.FindOrphanLaps(noHeader));
        Assert.Empty(RunRepair.FindOrphanLaps(allEmpty));
        Assert.Empty(RunRepair.RepairAll(tmp.Path));
    }

    [Fact]
    public void Empty_runs_are_classified_and_not_treated_as_corrupt()
    {
        using var tmp = new TempDir();
        var empty = CreateRun(tmp, @"FP\AA01\Run_1039", data: [], empty: [], referenced: []);
        var corrupt = CreateRun(tmp, @"FP\AA01\Run_1038", data: [], empty: ["Lap_1"], referenced: []);

        var skipped = RunFolderInspector.FindSkipped(tmp.Path, []);

        Assert.True(skipped.Single(s => s.RunPath == empty).IsEmpty);
        Assert.False(skipped.Single(s => s.RunPath == corrupt).IsEmpty);
        Assert.Contains("empty run", skipped.Single(s => s.RunPath == empty).Reason);
    }

    // ------------------------------------------------------------------ status: revision and length pending

    [Fact]
    public void Status_states_drive_reprocessing()
    {
        using var tmp = new TempDir();
        Assert.Equal(OracleStatus.State.NotProcessed, OracleStatus.GetState(tmp.Path));

        tmp.Write("oracle.status.json", """{ "version": "1.0", "exitCode": 0, "steps": { "stint": { "status": "success" } } }""");
        Assert.Equal(OracleStatus.State.Outdated, OracleStatus.GetState(tmp.Path)); // processed before revisions existed
        Assert.False(OracleStatus.IsAlreadyProcessed(tmp.Path));

        var status = OracleStatus.Create(tmp.Path);
        status.SetStep("stint", StepStatus.Success);
        status.ExitCode = (int)OracleExitCode.TrackLengthMissing;
        status.Save(tmp.Path);
        Assert.Equal(OracleStatus.State.LengthPending, OracleStatus.GetState(tmp.Path));
        Assert.True(OracleStatus.IsAlreadyProcessed(tmp.Path));

        status.ExitCode = (int)OracleExitCode.Success;
        status.Save(tmp.Path);
        Assert.Equal(OracleStatus.State.Done, OracleStatus.GetState(tmp.Path));
    }

    // ------------------------------------------------------------------ rejected suggestions

    [Fact]
    public void Rejected_length_is_ignored_in_circuits_and_hed_and_not_measured_again()
    {
        using var tmp = new TempDir();
        var kompass = tmp.Write("KOMPASS.json", """{ "MX": { "Tracks": [ { "Track": "Teutschenthal", "Countrycode": "GER" } ] } }""");
        var workbook = Path.Combine(tmp.Path, "loadChannels.xlsx");
        using (var wb = new XLWorkbook())
        {
            var sheet = wb.Worksheets.Add("CIRCUITS");
            sheet.Cell(1, 1).Value = "Name";
            sheet.Cell(2, 1).Value = "Teutschenthal";
            sheet.Cell(2, 9).Value = 1570;
            wb.SaveAs(workbook);
        }

        var suggestions = Path.Combine(tmp.Path, "suggestions");
        var store = new TrackLengthSuggestionStore(suggestions);
        store.Write(new TrackLengthSuggestion { TrackName = "Teutschenthal", LengthMeters = 1570, Source = "ORACLE GPS" });
        store.MarkRejected(store.ReadPending()[0]);

        var gate = new TrackLengthGate(kompass, workbook, new TrackLengthSettings { SuggestionsDirectory = suggestions, LockWaitSeconds = 0 });
        var known = gate.ResolveKnown("Teutschenthal", "1570");
        Assert.Equal(TrackLengthGateStatus.Missing, known.Status);
        Assert.Contains(known.Warnings, w => w.Contains("rejected"));

        var metadata = new RunMetadata { TrackName = "Teutschenthal", CountryCode = "GER" };
        var again = gate.Complete(known, metadata, [[0, 1568, 1570, 1572, 0]], "run");
        Assert.Equal(TrackLengthGateStatus.Missing, again.Status);
        Assert.Contains("rejected in KODE", again.Message);

        var better = gate.Complete(known, metadata, [[0, 1563, 1565, 1567, 1564, 0]], "run");
        Assert.Equal(1565, better.LengthMeters); // median in whole meters, written to CIRCUITS
        Assert.Contains(new LoadChannelsWriter().ReadCircuits(new LoadChannelsOptions { WorkbookPath = workbook }),
            c => c.Name == "Teutschenthal" && c.LengthMeters == 1565);
    }

    // ------------------------------------------------------------------ concurrency and locks

    [Fact]
    public void ProcessingLock_is_exclusive_and_removed_after_use()
    {
        using var tmp = new TempDir();
        using (var first = ProcessingLock.TryAcquire(tmp.Path))
        {
            Assert.NotNull(first);
            Assert.Null(ProcessingLock.TryAcquire(tmp.Path));
        }

        Assert.False(File.Exists(Path.Combine(tmp.Path, ProcessingLock.FileName)));
        using var again = ProcessingLock.TryAcquire(tmp.Path);
        Assert.NotNull(again);
    }

    [Fact]
    public void WorkbookGuard_retries_while_the_workbook_is_locked()
    {
        var previous = WorkbookGuard.RetryInterval;
        WorkbookGuard.RetryInterval = TimeSpan.FromMilliseconds(20);
        try
        {
            var calls = 0;
            var result = WorkbookGuard.WriteWithRetry(() => ++calls < 3
                ? LoadChannelsSyncResult.Locked("open in Excel")
                : LoadChannelsSyncResult.Updated(0, 1, 0), TimeSpan.FromSeconds(5));
            Assert.Equal(LoadChannelsSyncStatus.Updated, result.Status);
            Assert.Equal(3, calls);

            var noWait = WorkbookGuard.WriteWithRetry(() => LoadChannelsSyncResult.Locked("open"), TimeSpan.Zero);
            Assert.Equal(LoadChannelsSyncStatus.Locked, noWait.Status);
        }
        finally
        {
            WorkbookGuard.RetryInterval = previous;
        }
    }

    // ------------------------------------------------------------------ helpers for background operation

    [Fact]
    public void LapHistory_reads_own_lap_distances_without_the_processed_filter()
    {
        using var tmp = new TempDir();
        var run = tmp.CreateDir(@"S\AA01\Run_1");
        for (var i = 0; i < 3; i++)
        {
            var lap = tmp.CreateDir($@"S\AA01\Run_1\Lap_{i}");
            File.WriteAllText(Path.Combine(lap, "LapHeader.xml"), $"<LapHeader><LapDistance>{(i == 0 ? 0 : 1500 + i)}</LapDistance></LapHeader>");
        }

        Assert.Equal([1501, 1502], Assert.Single(LapHistory.ReadLapDistances([run])));
    }

    [Fact]
    public void Old_logs_are_deleted()
    {
        using var tmp = new TempDir();
        var old = tmp.Write("oracle-20260101-000000.log", "old");
        File.SetLastWriteTime(old, DateTime.Now.AddDays(-90));
        var recent = tmp.Write("oracle-20261012-000000.log", "new");

        Assert.Equal(1, OracleLogger.DeleteOldLogs(tmp.Path, TimeSpan.FromDays(60)));
        Assert.False(File.Exists(old));
        Assert.True(File.Exists(recent));
    }

    private static string CreateRun(TempDir tmp, string relative, string[] data, string[] empty, string[]? referenced)
    {
        var run = tmp.CreateDir(relative);
        foreach (var lap in data)
            File.WriteAllText(Path.Combine(tmp.CreateDir(Path.Combine(relative, lap)), "cableData.ztx"), "x");
        foreach (var lap in empty)
            File.WriteAllText(Path.Combine(tmp.CreateDir(Path.Combine(relative, lap)), "LapHeader.xml"), "<LapHeader/>");
        if (referenced is not null)
        {
            var laps = string.Concat(referenced.Select(l => $"<Lap><LapInfo><Path>{l}\\cableData.ztx</Path></LapInfo></Lap>"));
            File.WriteAllText(Path.Combine(run, "RunLapHeader.xml"), $"<RunLapHeader><Run><LapArray>{laps}</LapArray></Run></RunLapHeader>");
        }
        return Path.GetFullPath(run);
    }
}
