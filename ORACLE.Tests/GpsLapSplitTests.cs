using System.Xml.Linq;
using Ktm.Racing.Domain.Geo;
using Ktm.Racing.Domain.Telemetry;
using IOPath = System.IO.Path;

namespace Ktm.Racing.Oracle.Tests;

/// <summary>A WinTAX-like run folder on disk (headers, lap folders) for the file side of the split.</summary>
internal static class FakeRun
{
    public const string StartWall = "13/09/2026 13:14:05.250";

    /// <summary>Creates <c>Run_885</c> with the given laps (folder name, acquisition) and a RunLapHeader.xml.</summary>
    public static string Create(TempDir tmp, params (string Folder, string Abs)[] laps)
    {
        var run = IOPath.Combine(tmp.Path, "R1", "AA01", "Run_885");
        Directory.CreateDirectory(run);
        File.WriteAllText(IOPath.Combine(run, "Car.xml"), "<Car/>");
        File.WriteAllText(IOPath.Combine(run, "Session.xml"), "<Session/>");

        var entries = new List<XElement>();
        foreach (var (folder, abs) in laps)
        {
            var lap = IOPath.Combine(run, folder);
            Directory.CreateDirectory(lap);
            File.WriteAllBytes(IOPath.Combine(lap, "cableData.ztx"), new byte[1000]);
            File.WriteAllText(IOPath.Combine(lap, "BitmappedChannels.xml"), "<BitmappedChannels/>");
            File.WriteAllText(IOPath.Combine(lap, "EnumChannels.xml"), "<EnumChannels/>");
            new XDocument(new XElement("LapHeader",
                new XElement("Ver", 3), new XElement("Alias", "AA01"), new XElement("StartTime", "13/09/2026 13:14:05"),
                new XElement("Table", new XElement("Name", "260605_v007"), new XElement("Version", 18)),
                new XElement("Marker"), new XElement("Abs", abs), new XElement("Run", 885), new XElement("Lap", 1),
                new XElement("LapTime", "1503.469"),
                new XElement("StartTimeInfo", new XElement("InfoArray", new XElement("StartTimeInfo",
                    new XElement("Acq", "cable"), new XElement("STMs", StartWall), new XElement("STS", 1789305245), new XElement("STNs", 250000000)))),
                new XElement("SerialNumber", 412), new XElement("LapSize", 89560424),
                new XElement("OpenType", "TriggerIn"), new XElement("CloseType", "Reset"))).Save(IOPath.Combine(lap, "LapHeader.xml"));

            entries.Add(new XElement("Lap",
                new XElement("LapInfo", new XElement("ModT", 1), new XElement("Path", $"{folder}\\cableData.ztx"), new XElement("AcqType", 0), new XElement("LapIndex", 0)),
                new XElement("Infos",
                    Info("CarName", "AA01"), Info("Abs", abs), Info("LapTime", "15034690"), Info("LapTimeMarker", "15034690"),
                    Info("CronoTime", "1503.47"), Info("StartTimeMSec", "13:14:05.250"), Info("StartTimeS", "1789305245"),
                    Info("StartTimeNs", "250000000"), Info("StartTime", "1789305245"), Info("OpenType", "TriggerIn"),
                    Info("CloseType", "Reset"), Info("FilePath", $"{folder}\\"))));
        }

        new XDocument(new XElement("RunLapHeader",
            new XElement("Inf", new XElement("Path", run + "\\"), new XElement("Ver", 14), new XElement("ModT", 1), new XElement("CheckedlInfo", 0)),
            new XElement("Run", new XElement("LapArray", entries)))).Save(IOPath.Combine(run, "RunLapHeader.xml"));
        return run;
    }

    /// <summary>Simulates the API output: <c>&lt;out&gt;\Track\Session\Device\Run_000\Lap_0000000k</c> with header-less laps.</summary>
    public static string WriteApiOutput(string output, int laps, bool withData = true)
    {
        var run = IOPath.Combine(output, "Track", "Session", "Device", "Run_000");
        for (var i = 1; i <= laps; i++)
        {
            var lap = IOPath.Combine(run, $"Lap_{i:00000000}");
            Directory.CreateDirectory(lap);
            File.WriteAllBytes(IOPath.Combine(lap, "cableData.ztx"), withData ? new byte[100] : []);
            new XDocument(new XElement("LapHeader", new XElement("Alias"), new XElement("Abs", i), new XElement("LapTime", "0"),
                new XElement("StartTimeInfo", new XElement("InfoArray", new XElement("StartTimeInfo",
                    new XElement("STMs", "20/10/2026 15:29:33.000"), new XElement("STS", 1792510173), new XElement("STNs", 0)))))).Save(IOPath.Combine(lap, "LapHeader.xml"));
        }

        File.WriteAllText(IOPath.Combine(run, "Car.xml"), "<Car/>");
        return run;
    }

    public static string[] LapFolders(string run) =>
        Directory.GetDirectories(run, "Lap_*", SearchOption.TopDirectoryOnly).Select(d => IOPath.GetFileName(d)!).OrderBy(n => n).ToArray();

    private static XElement Info(string name, string data) => new("Info", new XElement("Name", name), new XElement("Data", data), new XElement("Infos"));
}

/// <summary>Fake of the native data access: GPS from the stadium track, writes an API-like folder tree.</summary>
internal sealed class FakeSplitAccess : ISplitDataAccess
{
    public required IReadOnlyList<double> OriginalMarks { get; init; }
    public GpsTrace? Gps { get; init; }
    public string[] WriteErrors { get; set; } = [];
    public bool BreakFinalReadBack { get; set; }
    public double LatitudeFirstTimeAfterWrite { get; set; }
    public int WriteCalls { get; private set; }
    private IReadOnlyList<double> _written = [];

    public (IReadOnlyList<double> Marks, GpsTrace? Gps, string? Problem) ReadGps(string runPath, string latitudeChannel, string longitudeChannel) =>
        (MarksOf(runPath), Gps, Gps is null ? "GPS channels Lat_dez/Lon_dez not found" : null);

    public SplitWriteResult WriteSplit(string runPath, IReadOnlyList<double> marks, string outputDirectory)
    {
        WriteCalls++;
        _written = marks;
        if (WriteErrors.Length == 0)
            FakeRun.WriteApiOutput(outputDirectory, marks.Count - 1);
        return new SplitWriteResult(WriteErrors, [new ChannelStat("Lat_dez", 1000, 0.009, 50), new ChannelStat("TOil", 100, 0.069, 10)]);
    }

    public SplitReadBack ReadBack(string runPath, IReadOnlyCollection<string> channels)
    {
        var marks = runPath.Contains(LapSplitStore.TempFolderName, StringComparison.OrdinalIgnoreCase) ? _written
            : BreakFinalReadBack ? [0.0, 1.0] : MarksOf(runPath);
        return new SplitReadBack(marks, 2, [new ChannelStat("Lat_dez", 1000, LatitudeFirstTimeAfterWrite, 50), new ChannelStat("TOil", 101, 0.000, 10)]);
    }

    private IReadOnlyList<double> MarksOf(string runPath) =>
        FakeRun.LapFolders(runPath).Any(n => n.Contains("_Def_", StringComparison.Ordinal)) && _written.Count > 0 ? _written : OriginalMarks;
}

public sealed class LapSplitStoreTests
{
    [Fact]
    public void ReadLapFolders_follows_RunLapHeader_order_with_acquisition()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000916_00412", "916"), ("Lap_00000915_00412", "915"));

        var laps = LapSplitStore.ReadLapFolders(run);

        Assert.Equal(["Lap_00000916_00412", "Lap_00000915_00412"], laps.Select(l => l.Folder));
        Assert.Equal(["916", "915"], laps.Select(l => l.Acquisition));
    }

    [Fact]
    public void Commit_swaps_in_named_laps_with_complete_headers_and_backs_up_the_originals()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"));
        var written = FakeRun.WriteApiOutput(IOPath.Combine(run, LapSplitStore.TempFolderName), 3);
        double[] marks = [0, 51.52, 147.56, 1503.47];

        var manifest = LapSplitStore.Commit(run, written, marks, [new RecordedLap("Lap_00000894_00412", "894", 0, 1503.47)], new SplitManifest());

        Assert.Equal(["Lap_00000894_00412_Def_000", "Lap_00000894_00412_Def_001", "Lap_00000894_00412_Def_002"], FakeRun.LapFolders(run));
        var backup = LapSplitStore.FindLatestSplit(run).Backup!;
        Assert.True(File.Exists(IOPath.Combine(backup, "Lap_00000894_00412", "cableData.ztx")));
        Assert.True(File.Exists(IOPath.Combine(backup, "RunLapHeader.xml")));
        Assert.Equal(["Lap_00000894_00412"], manifest.OriginalLaps);

        var second = IOPath.Combine(run, "Lap_00000894_00412_Def_001");
        Assert.True(File.Exists(IOPath.Combine(second, "BitmappedChannels.xml")), "per-lap files the API does not write are copied");
        var header = XDocument.Load(IOPath.Combine(second, "LapHeader.xml")).Root!;
        Assert.Equal("AA01", header.Element("Alias")!.Value);
        Assert.Equal("894", header.Element("Abs")!.Value);
        Assert.Equal("96.04", header.Element("LapTime")!.Value);
        Assert.Equal("13/09/2026 13:14:56.770", header.Descendants("STMs").Single().Value);
        Assert.Equal("1789305296", header.Descendants("STS").Single().Value);
        Assert.Equal("770000000", header.Descendants("STNs").Single().Value);
        Assert.Equal("PCCommand", header.Element("OpenType")!.Value);
        Assert.Equal("PcCommand", header.Element("CloseType")!.Value);
        var first = XDocument.Load(IOPath.Combine(run, "Lap_00000894_00412_Def_000", "LapHeader.xml")).Root!;
        Assert.Equal("TriggerIn", first.Element("OpenType")!.Value);
        var last = XDocument.Load(IOPath.Combine(run, "Lap_00000894_00412_Def_002", "LapHeader.xml")).Root!;
        Assert.Equal("Reset", last.Element("CloseType")!.Value);

        var runLaps = XDocument.Load(IOPath.Combine(run, "RunLapHeader.xml")).Descendants("Lap").ToList();
        Assert.Equal(3, runLaps.Count);
        Assert.Equal(@"Lap_00000894_00412_Def_001\cableData.ztx", runLaps[1].Element("LapInfo")!.Element("Path")!.Value);
        string InfoOf(XElement lap, string name) => lap.Descendants("Info").Single(i => i.Element("Name")!.Value == name).Element("Data")!.Value;
        Assert.Equal("960400", InfoOf(runLaps[1], "LapTime"));
        Assert.Equal("13:14:56.770", InfoOf(runLaps[1], "StartTimeMSec"));
        Assert.Equal(@"Lap_00000894_00412_Def_001\", InfoOf(runLaps[1], "FilePath"));
        Assert.Equal(RunRepair.ReferencedLaps(run)!.OrderBy(n => n), FakeRun.LapFolders(run));
    }

    [Fact]
    public void Commit_of_two_acquisitions_keeps_each_lap_with_its_acquisition()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000915_00412", "915"), ("Lap_00000916_00412", "916"));
        var written = FakeRun.WriteApiOutput(IOPath.Combine(run, LapSplitStore.TempFolderName), 3);

        LapSplitStore.Commit(run, written, [0, 100, 200, 300],
            [new RecordedLap("Lap_00000915_00412", "915", 0, 150), new RecordedLap("Lap_00000916_00412", "916", 150, 300)], new SplitManifest());

        Assert.Equal(["Lap_00000915_00412_Def_000", "Lap_00000915_00412_Def_001", "Lap_00000916_00412_Def_002"], FakeRun.LapFolders(run));
        var third = XDocument.Load(IOPath.Combine(run, "Lap_00000916_00412_Def_002", "LapHeader.xml")).Root!;
        Assert.Equal("916", third.Element("Abs")!.Value);
        Assert.Equal("13/09/2026 13:14:55.250", third.Descendants("STMs").Single().Value); // 200 s − 150 s into acquisition 916
    }

    [Fact]
    public void Commit_failure_rolls_back_to_the_original_run()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"));
        var originalHeader = File.ReadAllText(IOPath.Combine(run, "RunLapHeader.xml"));
        // Damage the RunLapHeader after reading: the commit fails after the laps were already moved.
        var written = FakeRun.WriteApiOutput(IOPath.Combine(run, LapSplitStore.TempFolderName), 2);
        File.WriteAllText(IOPath.Combine(run, "RunLapHeader.xml"), "<RunLapHeader><Inf/></RunLapHeader>");

        Assert.ThrowsAny<Exception>(() => LapSplitStore.Commit(run, written, [0, 50, 100],
            [new RecordedLap("Lap_00000894_00412", "894", 0, 100)], new SplitManifest()));

        Assert.Equal(["Lap_00000894_00412"], FakeRun.LapFolders(run));
        Assert.True(File.Exists(IOPath.Combine(run, "Lap_00000894_00412", "cableData.ztx")));
        Assert.Equal("<RunLapHeader><Inf/></RunLapHeader>", File.ReadAllText(IOPath.Combine(run, "RunLapHeader.xml")).Trim());
        Assert.False(Directory.Exists(IOPath.Combine(run, LapSplitStore.BackupFolderName)) &&
                     Directory.GetDirectories(IOPath.Combine(run, LapSplitStore.BackupFolderName)).Length > 0);
        Assert.NotEmpty(originalHeader);
    }

    [Fact]
    public void Commit_rejects_incomplete_api_output_without_touching_the_run()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"));
        var written = FakeRun.WriteApiOutput(IOPath.Combine(run, LapSplitStore.TempFolderName), 2, withData: false);

        Assert.Throws<InvalidDataException>(() => LapSplitStore.Commit(run, written, [0, 50, 100],
            [new RecordedLap("Lap_00000894_00412", "894", 0, 100)], new SplitManifest()));
        Assert.Throws<InvalidDataException>(() => LapSplitStore.Commit(run, written, [0, 30, 60, 100],
            [new RecordedLap("Lap_00000894_00412", "894", 0, 100)], new SplitManifest()));

        Assert.Equal(["Lap_00000894_00412"], FakeRun.LapFolders(run));
        Assert.False(Directory.Exists(IOPath.Combine(run, LapSplitStore.BackupFolderName)));
    }

    [Fact]
    public void Undo_restores_the_original_laps_once()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"));
        var header = File.ReadAllText(IOPath.Combine(run, "RunLapHeader.xml"));
        var written = FakeRun.WriteApiOutput(IOPath.Combine(run, LapSplitStore.TempFolderName), 2);
        LapSplitStore.Commit(run, written, [0, 50, 100], [new RecordedLap("Lap_00000894_00412", "894", 0, 100)], new SplitManifest());

        var undo = LapSplitStore.Undo(run);

        Assert.True(undo.Success, undo.Message);
        Assert.Equal(["Lap_00000894_00412"], FakeRun.LapFolders(run));
        Assert.Equal(header, File.ReadAllText(IOPath.Combine(run, "RunLapHeader.xml")));
        Assert.False(LapSplitStore.Undo(run).Success);
    }

    [Fact]
    public void Undo_refuses_when_the_laps_were_changed_after_the_split()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"));
        var written = FakeRun.WriteApiOutput(IOPath.Combine(run, LapSplitStore.TempFolderName), 2);
        LapSplitStore.Commit(run, written, [0, 50, 100], [new RecordedLap("Lap_00000894_00412", "894", 0, 100)], new SplitManifest());
        Directory.CreateDirectory(IOPath.Combine(run, "Lap_00000894_00412_Def_001_Def_000"));

        var undo = LapSplitStore.Undo(run);

        Assert.False(undo.Success);
        Assert.Contains("changed", undo.Message);
        Assert.Equal(3, FakeRun.LapFolders(run).Length);
    }

    [Theory]
    [InlineData("Lap_00000894_00412", true)]
    [InlineData(@"..\Lap_x", false)]
    [InlineData(@"Lap_1\..\..\Windows", false)]
    [InlineData("Run_885", false)]
    [InlineData("", false)]
    public void Only_plain_lap_folder_names_are_acted_upon(string name, bool safe) =>
        Assert.Equal(safe, LapSplitStore.IsSafeLapFolderName(name));

    [Fact]
    public void Run_discovery_ignores_ORACLE_work_folders()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"));
        FakeRun.WriteApiOutput(IOPath.Combine(IOPath.GetDirectoryName(run)!, "_ORACLE_split_tmp"), 1);

        Assert.Equal([run], RunFolderInspector.FindRunFolders(tmp.Path));
    }
}

public sealed class GpsLapSplitServiceTests
{
    private static readonly GeoCoordinate TrackPoint = StadiumTrackPoint();

    [Fact]
    public void Splits_a_single_lap_run_and_is_idempotent()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"));
        var access = new FakeSplitAccess { OriginalMarks = [0, 200], Gps = Stadium.Drive(200) };
        var service = new GpsLapSplitService(access: access, freeSpace: _ => long.MaxValue);

        var (line, outcomes) = service.SplitRuns([run], TrackPoint);

        Assert.Equal(FinishLineBuildStatus.Success, line.Status);
        var outcome = Assert.Single(outcomes);
        Assert.Equal(GpsLapSplitOutcomeStatus.Split, outcome.Status);
        Assert.Equal(LapSplitVerdict.NotSplit, outcome.Verdict);
        Assert.Equal(0.0, outcome.Marks[0]);
        Assert.Equal(200.0, outcome.Marks[^1]);
        Assert.Equal(outcome.LapCount, FakeRun.LapFolders(run).Length);
        Assert.Equal(69, outcome.MaxTimeShiftMs, 3);
        Assert.False(Directory.Exists(IOPath.Combine(run, LapSplitStore.TempFolderName)));

        // A second run (also with --force) recognizes its own split and leaves the run alone.
        var again = Assert.Single(service.SplitRuns([run], TrackPoint).Outcomes);
        Assert.Equal(GpsLapSplitOutcomeStatus.Skipped, again.Status);
        Assert.Equal(LapSplitVerdict.AlreadySplit, again.Verdict);
        Assert.Equal(1, access.WriteCalls);
    }

    [Fact]
    public void Dry_run_writes_nothing()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"));
        var access = new FakeSplitAccess { OriginalMarks = [0, 200], Gps = Stadium.Drive(200) };

        var outcome = Assert.Single(new GpsLapSplitService(access: access).SplitRuns([run], TrackPoint, dryRun: true).Outcomes);

        Assert.Equal(GpsLapSplitOutcomeStatus.Planned, outcome.Status);
        Assert.True(outcome.LapCount >= 2);
        Assert.Equal(0, access.WriteCalls);
        Assert.Equal(["Lap_00000894_00412"], FakeRun.LapFolders(run));
    }

    [Fact]
    public void WinTAX_split_run_is_left_alone()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000879_00412_Def_000", "879"), ("Lap_00000879_00412_Def_001", "879"));
        var access = new FakeSplitAccess { OriginalMarks = [0, 100, 200], Gps = Stadium.Drive(200) };

        var outcome = Assert.Single(new GpsLapSplitService(access: access).SplitRuns([run], TrackPoint).Outcomes);

        Assert.Equal(LapSplitVerdict.AlreadySplit, outcome.Verdict);
        Assert.Equal(GpsLapSplitOutcomeStatus.Skipped, outcome.Status);
        Assert.NotNull(outcome.ExistingBoundaryDeviationSeconds);
        Assert.Equal(0, access.WriteCalls);
    }

    [Fact]
    public void Write_errors_leave_the_run_unchanged()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"));
        var access = new FakeSplitAccess { OriginalMarks = [0, 200], Gps = Stadium.Drive(200), WriteErrors = ["lap 1: Invalid start offset value (33 channel(s))"] };

        var outcome = Assert.Single(new GpsLapSplitService(access: access, freeSpace: _ => long.MaxValue).SplitRuns([run], TrackPoint).Outcomes);

        Assert.Equal(GpsLapSplitOutcomeStatus.Failed, outcome.Status);
        Assert.Contains("Invalid start offset", outcome.Message);
        Assert.Equal(["Lap_00000894_00412"], FakeRun.LapFolders(run));
        Assert.False(Directory.Exists(IOPath.Combine(run, LapSplitStore.TempFolderName)));
    }

    [Fact]
    public void Failed_read_back_after_the_swap_rolls_back()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"));
        var access = new FakeSplitAccess { OriginalMarks = [0, 200], Gps = Stadium.Drive(200), BreakFinalReadBack = true };

        var outcome = Assert.Single(new GpsLapSplitService(access: access, freeSpace: _ => long.MaxValue).SplitRuns([run], TrackPoint).Outcomes);

        Assert.Equal(GpsLapSplitOutcomeStatus.Failed, outcome.Status);
        Assert.Contains("rolled back", outcome.Message);
        Assert.Equal(["Lap_00000894_00412"], FakeRun.LapFolders(run));
    }

    [Fact]
    public void Channel_shift_beyond_one_sample_period_aborts_before_anything_is_swapped()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"));
        var access = new FakeSplitAccess { OriginalMarks = [0, 200], Gps = Stadium.Drive(200), LatitudeFirstTimeAfterWrite = 0.1 };

        var outcome = Assert.Single(new GpsLapSplitService(access: access, freeSpace: _ => long.MaxValue).SplitRuns([run], TrackPoint).Outcomes);

        Assert.Equal(GpsLapSplitOutcomeStatus.Failed, outcome.Status);
        Assert.Contains("sample periods", outcome.Message);
        Assert.Equal(["Lap_00000894_00412"], FakeRun.LapFolders(run));
        Assert.False(Directory.Exists(IOPath.Combine(run, LapSplitStore.BackupFolderName)));
    }

    [Fact]
    public void Locked_run_is_not_touched_by_the_stand_alone_command()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"));
        var access = new FakeSplitAccess { OriginalMarks = [0, 200], Gps = Stadium.Drive(200) };
        using var other = ProcessingLock.TryAcquire(run);

        var outcome = Assert.Single(new GpsLapSplitService(access: access, freeSpace: _ => long.MaxValue, lockRuns: true).SplitRuns([run], TrackPoint).Outcomes);

        Assert.Equal(GpsLapSplitOutcomeStatus.Failed, outcome.Status);
        Assert.Contains("another ORACLE process", outcome.Message);
        Assert.Equal(0, access.WriteCalls);
    }

    [Fact]
    public void Not_enough_disk_space_writes_nothing()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"));
        var access = new FakeSplitAccess { OriginalMarks = [0, 200], Gps = Stadium.Drive(200) };

        var outcome = Assert.Single(new GpsLapSplitService(access: access, freeSpace: _ => 1024).SplitRuns([run], TrackPoint).Outcomes);

        Assert.Equal(GpsLapSplitOutcomeStatus.Failed, outcome.Status);
        Assert.Contains("disk space", outcome.Message);
        Assert.Equal(0, access.WriteCalls);
    }

    [Fact]
    public void Missing_gps_and_far_track_point_are_reported_without_writing()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"));
        var noGps = new FakeSplitAccess { OriginalMarks = [0, 200] };
        var (line, outcomes) = new GpsLapSplitService(access: noGps).SplitRuns([run], TrackPoint);
        Assert.Equal(FinishLineBuildStatus.NoPasses, line.Status);
        Assert.Equal(GpsLapSplitOutcomeStatus.Skipped, Assert.Single(outcomes).Status);

        var far = new FakeSplitAccess { OriginalMarks = [0, 200], Gps = Stadium.Drive(200) };
        var farResult = new GpsLapSplitService(access: far).SplitRuns([run], new GeoCoordinate(TrackPoint.Latitude + 0.01, TrackPoint.Longitude));
        Assert.Contains("KOMPASS", farResult.Line.Message);
        Assert.Equal(0, far.WriteCalls);
    }

    [Fact]
    public void Run_without_any_gps_fix_is_reported_and_not_split()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"));
        var noFix = new GpsTrace([0.0, 0.02, 0.04], [0.0, 0.0, 0.0], [0.0, 0.0, 0.0]);
        var service = new GpsLapSplitService(access: new FakeSplitAccess { OriginalMarks = [0, 200], Gps = noFix });

        var analysis = service.Analyze(run);

        Assert.Null(analysis.Gps);
        Assert.Equal("no GPS fix in this run", analysis.Problem);
    }

    [Fact]
    public void Inconsistent_lap_structure_is_not_split()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"));
        var access = new FakeSplitAccess { OriginalMarks = [0, 100, 200], Gps = Stadium.Drive(200) };

        var outcome = Assert.Single(new GpsLapSplitService(access: access).SplitRuns([run], TrackPoint).Outcomes);

        Assert.Equal(GpsLapSplitOutcomeStatus.Skipped, outcome.Status);
        Assert.Contains("inconsistent", outcome.Message);
    }

    private static GeoCoordinate StadiumTrackPoint() => Stadium.Projection.ToGeo(0, -8);
}

/// <summary>Same stadium track as in the domain tests (bottom straight y = 0, top straight y = 30).</summary>
internal static class Stadium
{
    public static readonly LocalProjection Projection = new(new GeoCoordinate(48.0, 11.0));
    private static readonly double LapLength = 400 + 2 * Math.PI * 15;

    public static GpsTrace Drive(double seconds, double hz = 10, double speed = 15)
    {
        var n = (int)(seconds * hz) + 1;
        var time = new double[n];
        var lat = new double[n];
        var lon = new double[n];
        for (var i = 0; i < n; i++)
        {
            time[i] = i / hz;
            var (x, y) = At(50 + speed * time[i]);
            var g = Projection.ToGeo(x, y);
            (lat[i], lon[i]) = (g.Latitude, g.Longitude);
        }

        return new GpsTrace(time, lat, lon);
    }

    private static (double X, double Y) At(double s)
    {
        s %= LapLength;
        var arc = Math.PI * 15;
        if (s < 200) return (-100 + s, 0);
        s -= 200;
        if (s < arc) { var a = -Math.PI / 2 + s / 15; return (100 + 15 * Math.Cos(a), 15 + 15 * Math.Sin(a)); }
        s -= arc;
        if (s < 200) return (100 - s, 30);
        s -= 200;
        var b = Math.PI / 2 + s / 15;
        return (-100 + 15 * Math.Cos(b), 15 + 15 * Math.Sin(b));
    }
}
