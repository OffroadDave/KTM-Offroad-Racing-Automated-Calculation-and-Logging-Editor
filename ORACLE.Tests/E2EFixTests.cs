using Ktm.Racing.Domain.Kompass;

namespace Ktm.Racing.Oracle.Tests;

/// <summary>Fixes from the E2E test RT22 (C1/C2/H1/M1/M2/M3/L1/L3/L4).</summary>
public sealed class E2EFixTests
{
    private static readonly Dictionary<string, string> Teams = new(StringComparer.OrdinalIgnoreCase)
    {
        ["01_KTM-FR"] = "KTM Factory Racing",
        ["02_HQV"] = "Husqvarna Factory Racing",
    };

    // ------------------------------------------------------------------ H1: path metadata by pattern

    [Theory]
    [InlineData(@"C:\MXTrackData\Testdata\01_KTM-FR\Wintax\2026\450\05_RT\RT22_GER-Teutschenthal\FP\AA01\Run_1032")]
    [InlineData(@"C:\MXTrackData\01_KTM-FR\Wintax\2026\450\05_RT\RT22_GER-Teutschenthal\FP\AA01\Run_1032")]
    [InlineData(@"D:\Data\01_KTM-FR\2026\RT22_GER-Teutschenthal\FP\AA01\Run_1032")]
    public void PathMetadata_is_found_by_pattern_at_any_depth(string runPath)
    {
        var parsed = PathMetadataParser.Parse(runPath, "RT22_GER-Teutschenthal", Teams, ["MX", "SX"]);

        Assert.Equal("KTM Factory Racing", parsed.TeamName);
        Assert.Equal("2026", parsed.SeasonName);
        Assert.Equal(string.Empty, parsed.DisciplineName);
    }

    [Fact]
    public void PathMetadata_reads_discipline_and_engine_class_above_the_event_only()
    {
        var parsed = PathMetadataParser.Parse(@"C:\Data\02_HQV\SX\2025\250\RT01_USA-Anaheim\R1\KW01\Run_1",
            "RT01_USA-Anaheim", Teams, ["MX", "SX"]);

        Assert.Equal(("Husqvarna Factory Racing", "2025", "SX", "250"),
            (parsed.TeamName, parsed.SeasonName, parsed.DisciplineName, parsed.EngineClass));
    }

    [Fact]
    public async Task KompassService_resolves_rider_and_discipline_for_the_example_layout()
    {
        using var tmp = new TempDir();
        var service = new KompassService();
        Assert.True(await service.LoadAsync(tmp.Write("KOMPASS.json", """
        { "MX": {
            "Tracks": [ { "Track": "Teutschenthal", "Country": "Germany", "Countrycode": "GER", "Tracklength": "1560", "Surface": "Hardpack" } ],
            "Riders": [ { "Rider": "Andrea Adamo", "Riderabbreviation": "AA", "Season": "2026", "Team": "KTM Factory Racing" } ] } }
        """)));

        var metadata = new RunMetadata { EventName = "RT22_GER-Teutschenthal", SessionName = "FP", BikeName = "AA01" };
        service.CompleteFromPathAndKompass(metadata,
            @"C:\MXTrackData\Testdata\01_KTM-FR\Wintax\2026\450\05_RT\RT22_GER-Teutschenthal\FP\AA01\Run_1032", Teams);

        Assert.Equal("Andrea Adamo", metadata.RiderName);
        Assert.Equal("KTM Factory Racing", metadata.TeamName);
        Assert.Equal("2026", metadata.SeasonName);
        Assert.Equal("MX", metadata.DisciplineName);
        Assert.Equal("450", metadata.EngineClass);
        Assert.Equal(("Teutschenthal", "GER", "1560", "Hardpack"), (metadata.TrackName, metadata.CountryCode, metadata.TrackLength, metadata.Surface));
        Assert.Equal("2026RT22_GER-Teutschenthal_AA01FP.HED", HedFileService.BuildHedFileName(metadata));
    }

    [Fact]
    public async Task KompassService_finds_rider_by_abbreviation_and_season_when_team_folder_is_unknown()
    {
        using var tmp = new TempDir();
        var service = new KompassService();
        await service.LoadAsync(tmp.Write("KOMPASS.json", """
        { "MX": { "Tracks": [], "Riders": [ { "Rider": "Andrea Adamo", "Riderabbreviation": "AA", "Season": "2026", "Team": "KTM Factory Racing" } ] } }
        """));

        var metadata = new RunMetadata { EventName = "RT22_GER-Teutschenthal", BikeName = "AA01" };
        service.CompleteFromPathAndKompass(metadata, @"C:\X\99_UNKNOWN\2026\RT22_GER-Teutschenthal\FP\AA01\Run_1", Teams);

        Assert.Equal("Andrea Adamo", metadata.RiderName);
        Assert.Equal("KTM Factory Racing", metadata.TeamName);
    }

    [Fact]
    public async Task KompassService_reports_invalid_file()
    {
        using var tmp = new TempDir();
        var service = new KompassService();

        Assert.False(await service.LoadAsync(tmp.Write("KOMPASS.json", "{ \"MX\": [")));
        Assert.True(service.LastLoadInvalid);
        Assert.False(await service.LoadAsync(Path.Combine(tmp.Path, "missing.json")));
        Assert.False(service.LastLoadInvalid);
    }

    [Fact]
    public void Hed_lines_are_corrected_from_metadata_but_empty_values_are_kept()
    {
        string[] lines = ["[KTM_NAMING]", "seasonName = Wintax", "teamName = Testdata", "riderName = ", "bikeName = AA 01", "trackLength = 1560"];
        var metadata = new RunMetadata { SeasonName = "2026", TeamName = "KTM Factory Racing", RiderName = "Andrea Adamo", BikeName = "AA01" };

        var result = HedFileService.ApplyMetadataToLines(lines, metadata);

        Assert.Equal(["[KTM_NAMING]", "seasonName = 2026", "teamName = KTM Factory Racing", "riderName = Andrea Adamo", "bikeName = AA01", "trackLength = 1560"], result);
    }

    // ------------------------------------------------------------------ C2: skipped run folders

    [Fact]
    public void Corrupt_run_folders_are_found_and_diagnosed()
    {
        using var tmp = new TempDir();
        var good = CreateRun(tmp, @"FP\AA01\Run_1", ["Lap_0001"]);
        var corrupt = CreateRun(tmp, @"FP\AA01\Run_2", ["Lap_0002_Def_000", "Lap_0002_Def_000_Def_001"], emptyLaps: ["Lap_0002_Def_000"]);

        var skipped = Assert.Single(RunFolderInspector.FindSkipped(Path.Combine(tmp.Path, "FP"), [good]));

        Assert.Equal(corrupt, skipped.RunPath);
        Assert.Contains("1 of 2 lap folder(s) without cableData.ztx", skipped.Reason);
        Assert.Contains("split several times", skipped.Reason);
        Assert.Equal([Path.GetFullPath(corrupt)], RunFolderInspector.FindRunFolders(corrupt));
    }

    [Fact]
    public void Status_with_only_skipped_runs_counts_as_processed()
    {
        using var tmp = new TempDir();
        var status = OracleStatus.Create(tmp.Path);
        status.SetStep("discovery", StepStatus.Partial);
        status.ExitCode = (int)OracleExitCode.PartialSuccess;
        status.Save(tmp.Path);
        Assert.True(OracleStatus.IsAlreadyProcessed(tmp.Path));

        status.SetStep("discovery", StepStatus.Success);
        status.Save(tmp.Path);
        Assert.False(OracleStatus.IsAlreadyProcessed(tmp.Path));
    }

    // ------------------------------------------------------------------ C1: exit code of several devices

    [Theory]
    [InlineData(new[] { OracleExitCode.Success, OracleExitCode.Success }, OracleExitCode.Success)]
    [InlineData(new[] { OracleExitCode.Success, OracleExitCode.PartialSuccess }, OracleExitCode.PartialSuccess)]
    [InlineData(new[] { OracleExitCode.PartialSuccess, OracleExitCode.TrackLengthMissing }, OracleExitCode.TrackLengthMissing)]
    [InlineData(new[] { OracleExitCode.TrackLengthMissing, OracleExitCode.ResourceLocked }, OracleExitCode.ResourceLocked)]
    [InlineData(new[] { OracleExitCode.InvalidData, OracleExitCode.Success }, OracleExitCode.InvalidData)]
    public void Exit_codes_of_several_devices_are_combined_by_severity(OracleExitCode[] codes, OracleExitCode expected) =>
        Assert.Equal(expected, OracleExitCodes.Combine(codes));

    // ------------------------------------------------------------------ M1: laps of other processed runs of the event

    [Fact]
    public void LapHistory_reads_processed_lap_distances_of_the_event()
    {
        using var tmp = new TempDir();
        var evt = Path.Combine(tmp.Path, "RT22_GER-Teutschenthal");
        var processed = CreateRun(tmp, @"RT22_GER-Teutschenthal\R2\AA01\Run_1096", ["Lap_1", "Lap_2", "Lap_3"], lapDistances: [690, 1561, 1559]);
        File.WriteAllText(Path.Combine(Path.GetDirectoryName(processed)!, "oracle.status.json"), "{}");
        var current = CreateRun(tmp, @"RT22_GER-Teutschenthal\FP\AA01\Run_1032", ["Lap_1"], lapDistances: [300]);
        CreateRun(tmp, @"RT22_GER-Teutschenthal\FP\AA01\Run_1033", ["Lap_1"]); // not processed yet
        CreateRun(tmp, @"RT22_GER-Teutschenthal\QR\AA01\Run_1062", ["Lap_1", "Lap_2"], lapDistances: [0, 1550]); // raw WinTAX values, no ORACLE status

        Assert.Equal(evt, LapHistory.FindEventFolder(current, "RT22_GER-Teutschenthal"));
        var laps = Assert.Single(LapHistory.ReadProcessedLapDistances(evt, [current]));
        Assert.Equal([690, 1561, 1559], laps);
        Assert.Contains(processed, RunFolderInspector.FindRunFolders(evt));
    }

    [Fact]
    public void Gate_uses_event_laps_when_the_own_run_has_none_and_reports_only_relevant_conflicts()
    {
        using var tmp = new TempDir();
        var kompass = tmp.Write("KOMPASS.json", """{ "MX": { "Tracks": [ { "Track": "Teutschenthal", "Countrycode": "GER" } ] } }""");
        var workbook = Path.Combine(tmp.Path, "loadChannels.xlsx");
        using (var wb = new ClosedXML.Excel.XLWorkbook())
        {
            var sheet = wb.Worksheets.Add("CIRCUITS");
            sheet.Cell(1, 1).Value = "Name";
            sheet.Cell(2, 1).Value = "Lommel"; sheet.Cell(2, 9).Value = 1000;
            sheet.Cell(3, 1).Value = "Loket"; sheet.Cell(3, 9).Value = 1000;
            wb.SaveAs(workbook);
        }

        var gate = new TrackLengthGate(kompass, workbook, new TrackLengthSettings { SuggestionsDirectory = Path.Combine(tmp.Path, "suggestions") });
        var metadata = new RunMetadata { TrackName = "Teutschenthal", CountryCode = "GER" };
        var known = gate.ResolveKnown("Teutschenthal", "");

        var withoutEvent = gate.Complete(known, metadata, [[0, 300, 0]], "run");
        Assert.Equal(TrackLengthGateStatus.Missing, withoutEvent.Status);

        var decision = gate.Complete(known, metadata, [[0, 300, 0]], "run",
            () => [new double[] { 690, 1561, 1559, 1562, 1558, 270 }]);

        Assert.Equal(TrackLengthGateStatus.Ok, decision.Status);
        Assert.Equal(1560, decision.LengthMeters);
        Assert.Contains("other processed run", decision.Message);
        Assert.DoesNotContain(decision.Warnings, w => w.Contains("Lommel"));
    }

    // ------------------------------------------------------------------ M3: input fingerprint

    [Fact]
    public void InputFingerprint_describes_file_hash_and_kode_metadata()
    {
        using var tmp = new TempDir();
        var file = tmp.Write("KOMPASS.json", "{}");
        tmp.Write("KOMPASS.meta.json", """{ "lastUpdatedUtc": "2026-10-12T12:07:04Z", "updatedBy": "reicheltd" }""");

        var text = InputFingerprint.Describe(file);

        Assert.Contains("KOMPASS.json", text);
        Assert.Contains("sha256 44136fa355b3", text); // SHA-256 of "{}"
        Assert.Contains("saved by reicheltd 2026-10-12T12:07:04Z", text);
        Assert.Contains("(missing)", InputFingerprint.Describe(Path.Combine(tmp.Path, "nope.json")));
    }

    // ------------------------------------------------------------------ L4: command line

    [Theory]
    [InlineData(new[] { "process", "C:\\Run_1", "--force", "--gps-split" }, null)]
    [InlineData(new[] { "process", "C:\\Run_1", "--bogus" }, "Unbekannte Option: --bogus")]
    [InlineData(new[] { "process", "C:\\Session", "AA", "01" }, "Unerwartetes Argument: AA")]
    [InlineData(new[] { "sync-motodb", "--workbook", "C:\\x.xlsx" }, null)]
    [InlineData(new[] { "sync-motodb", "--workbook" }, "Option --workbook erwartet einen Wert.")]
    [InlineData(new[] { "validate", "C:\\Run_1", "--force" }, "Unbekannte Option: --force")]
    [InlineData(new[] { "C:\\Run_1", "--force" }, null)]
    public void CommandLine_rejects_unknown_options(string[] args, string? expected)
    {
        var error = CommandLine.Validate(args);
        if (expected is null)
            Assert.Null(error);
        else
            Assert.StartsWith(expected, error);
    }

    private static string CreateRun(TempDir tmp, string relative, string[] laps, string[]? emptyLaps = null, double[]? lapDistances = null)
    {
        var run = tmp.CreateDir(relative);
        foreach (var file in new[] { "Session.xml", "Car.xml", "RunLapHeader.xml" })
            File.WriteAllText(Path.Combine(run, file), "<x/>");
        for (var i = 0; i < laps.Length; i++)
        {
            var lap = Directory.CreateDirectory(Path.Combine(run, laps[i])).FullName;
            if (emptyLaps is null || !emptyLaps.Contains(laps[i]))
                File.WriteAllText(Path.Combine(lap, "cableData.ztx"), "x");
            if (lapDistances is not null)
                File.WriteAllText(Path.Combine(lap, "LapHeader.xml"),
                    $"<LapHeader><Lap>{i + 1}</Lap><LapDistance>{lapDistances[i].ToString(System.Globalization.CultureInfo.InvariantCulture)}</LapDistance></LapHeader>");
        }
        return Path.GetFullPath(run);
    }
}
