using System.Text;
using System.Xml.Linq;
using Ktm.Racing.Domain.Telemetry;
using IOPath = System.IO.Path;

namespace Ktm.Racing.Oracle.Tests;

/// <summary>Regression tests for the bugs fixed after the October 2026 code review (IDs refer to the review list).</summary>
public sealed class ReviewBugfixTests
{
    // ---- W09 / B17 / B26 / B27: atomic writes ----------------------------------------------------------------------

    [Fact]
    public void AtomicFile_replaces_the_target_and_leaves_no_temp_file()
    {
        using var tmp = new TempDir();
        var file = tmp.Write("a.json", "old");

        AtomicFile.WriteAllText(file, "new äöü");

        Assert.Equal("new äöü", File.ReadAllText(file, Encoding.UTF8));
        Assert.Equal([0x6E], File.ReadAllBytes(file)[..1]); // no BOM
        Assert.Single(Directory.GetFiles(tmp.Path));
    }

    [Fact]
    public void AtomicFile_keeps_the_old_file_when_writing_fails()
    {
        using var tmp = new TempDir();
        var file = tmp.Write("a.txt", "old");

        Assert.ThrowsAny<Exception>(() => AtomicFile.WriteAllLines(file, Lines()));

        Assert.Equal("old", File.ReadAllText(file));
        Assert.Single(Directory.GetFiles(tmp.Path));

        static IEnumerable<string> Lines()
        {
            yield return "first";
            throw new IOException("disk full");
        }
    }

    // ---- B14: crash-safe lap swap ----------------------------------------------------------------------------------

    [Fact]
    public void Interrupted_lap_swap_is_rolled_back_by_recovery()
    {
        using var tmp = new TempDir();
        var (run, header) = CreateInterruptedSplit(tmp);

        Assert.Null(LapSplitStore.FindLatestSplit(run).Manifest); // a pending swap is never "the latest split"
        var message = LapSplitStore.RecoverInterrupted(run);

        Assert.NotNull(message);
        Assert.Equal(["Lap_00000894_00412"], FakeRun.LapFolders(run));
        Assert.Equal(header, File.ReadAllText(IOPath.Combine(run, "RunLapHeader.xml")));
        Assert.Null(LapSplitStore.RecoverInterrupted(run)); // done once
    }

    [Fact]
    public void Analysis_only_reports_an_interrupted_lap_swap()
    {
        using var tmp = new TempDir();
        var (run, _) = CreateInterruptedSplit(tmp);
        var service = new GpsLapSplitService(access: new FakeSplitAccess { OriginalMarks = [0, 200], Gps = Stadium.Drive(200) });

        var analysis = service.Analyze(run);

        Assert.Empty(analysis.Laps);
        Assert.Contains("interrupted lap split", analysis.Problem);
        Assert.True(LapSplitStore.HasInterruptedSplit(run));
        Assert.Equal(["Lap_00000894_00412_Def_000"], FakeRun.LapFolders(run)); // nothing restored or deleted
    }

    [Fact]
    public void Interrupted_lap_swap_is_recovered_only_under_the_locks()
    {
        using var tmp = new TempDir();
        var (run, header) = CreateInterruptedSplit(tmp);

        using (ProcessingLock.TryAcquire(IOPath.GetDirectoryName(run)!))
        {
            var locked = LapSplitStore.RecoverInterruptedLocked([run]);
            Assert.Equal(new[] { run }, locked.Locked);
            Assert.False(locked.Complete);
            Assert.True(LapSplitStore.HasInterruptedSplit(run));
        }

        using (ProcessingLock.TryAcquire(run))
        {
            Assert.Equal(new[] { run }, LapSplitStore.RecoverInterruptedLocked([run]).Locked);
            Assert.True(LapSplitStore.HasInterruptedSplit(run));
        }

        var recovery = LapSplitStore.RecoverInterruptedLocked([run]);

        Assert.Equal(new[] { run }, recovery.Recovered);
        Assert.True(recovery.Complete);
        Assert.False(LapSplitStore.HasInterruptedSplit(run));
        Assert.Equal(["Lap_00000894_00412"], FakeRun.LapFolders(run));
        Assert.Equal(header, File.ReadAllText(IOPath.Combine(run, "RunLapHeader.xml")));
        Assert.Empty(LapSplitStore.RecoverInterruptedLocked([run]).Recovered);
    }

    [Fact]
    public void Recovery_keeps_an_original_lap_that_was_not_moved_yet()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"), ("Lap_00000894_00412_Def_000", "895"));
        var original = File.ReadAllBytes(IOPath.Combine(run, "Lap_00000894_00412_Def_000", "LapHeader.xml"));

        // Crash after writing the pending manifest, before any original was moved: the new lap
        // Lap_00000894_00412_Def_000 has the same name as an original that is still in place.
        var backup = Directory.CreateDirectory(IOPath.Combine(run, LapSplitStore.BackupFolderName, "20261021-080000Z")).FullName;
        File.Copy(IOPath.Combine(run, "RunLapHeader.xml"), IOPath.Combine(backup, "RunLapHeader.xml"));
        File.WriteAllText(IOPath.Combine(backup, LapSplitStore.ManifestFileName), System.Text.Json.JsonSerializer.Serialize(new SplitManifest
        {
            Pending = true,
            HadRunLapHeader = true,
            OriginalLaps = ["Lap_00000894_00412", "Lap_00000894_00412_Def_000"],
            CreatedLaps = ["Lap_00000894_00412_Def_000", "Lap_00000894_00412_Def_001"],
        }));

        LapSplitStore.RecoverInterrupted(run);

        Assert.Equal(["Lap_00000894_00412", "Lap_00000894_00412_Def_000"], FakeRun.LapFolders(run));
        Assert.Equal(original, File.ReadAllBytes(IOPath.Combine(run, "Lap_00000894_00412_Def_000", "LapHeader.xml")));
        Assert.False(LapSplitStore.HasInterruptedSplit(run));
    }

    /// <summary>State after a crash mid-swap: pending manifest, original lap already in the backup, one new lap in place.</summary>
    private static (string Run, string Header) CreateInterruptedSplit(TempDir tmp)
    {
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"));
        var header = File.ReadAllText(IOPath.Combine(run, "RunLapHeader.xml"));

        var backup = Directory.CreateDirectory(IOPath.Combine(run, LapSplitStore.BackupFolderName, "20261021-080000Z")).FullName;
        File.Copy(IOPath.Combine(run, "RunLapHeader.xml"), IOPath.Combine(backup, "RunLapHeader.xml"));
        Directory.Move(IOPath.Combine(run, "Lap_00000894_00412"), IOPath.Combine(backup, "Lap_00000894_00412"));
        Directory.CreateDirectory(IOPath.Combine(run, "Lap_00000894_00412_Def_000"));
        File.WriteAllText(IOPath.Combine(run, "RunLapHeader.xml"), "<broken/>");
        File.WriteAllText(IOPath.Combine(backup, LapSplitStore.ManifestFileName), System.Text.Json.JsonSerializer.Serialize(new SplitManifest
        {
            Pending = true,
            HadRunLapHeader = true,
            OriginalLaps = ["Lap_00000894_00412"],
            CreatedLaps = ["Lap_00000894_00412_Def_000", "Lap_00000894_00412_Def_001"],
        }));
        return (run, header);
    }

    [Fact]
    public void Completed_commit_leaves_no_pending_manifest()
    {
        using var tmp = new TempDir();
        var run = FakeRun.Create(tmp, ("Lap_00000894_00412", "894"));
        var written = FakeRun.WriteApiOutput(IOPath.Combine(run, LapSplitStore.TempFolderName), 2);

        LapSplitStore.Commit(run, written, [0, 50, 100], [new RecordedLap("Lap_00000894_00412", "894", 0, 100)], new SplitManifest());

        var (_, manifest) = LapSplitStore.FindLatestSplit(run);
        Assert.NotNull(manifest);
        Assert.False(manifest!.Pending);
        Assert.Equal(["Lap_00000894_00412_Def_000", "Lap_00000894_00412_Def_001"], manifest.CreatedLaps);
        Assert.Null(LapSplitStore.RecoverInterrupted(run));
    }

    // ---- B31: start time shift without inventing a time zone -------------------------------------------------------

    [Fact]
    public void Lap_start_wall_clock_is_shifted_without_deriving_a_unix_time()
    {
        using var tmp = new TempDir();
        var lap = tmp.CreateDir("lap");
        var template = IOPath.Combine(lap, "template.xml");
        new XDocument(new XElement("LapHeader", new XElement("LapTime", "0"), new XElement("StartTime", "x"),
            new XElement("StartTimeInfo", new XElement("InfoArray", new XElement("StartTimeInfo",
                new XElement("STMs", "13/09/2026 13:14:05.250"), new XElement("STS"), new XElement("STNs")))))).Save(template);
        var target = IOPath.Combine(lap, "LapHeader.xml");
        File.Copy(template, target);

        LapSplitStore.WriteLapHeader(template, target, new LapSplitStore.NewLap("Lap_x_Def_001", "Lap_x", 51.52, 147.56, 0, false, false));

        var info = XDocument.Load(target).Descendants("StartTimeInfo").Last();
        Assert.Equal("13/09/2026 13:14:56.770", info.Element("STMs")!.Value);
        Assert.Equal(string.Empty, info.Element("STS")!.Value);
    }

    // ---- B11: lap order = data order (start time), not RunLapHeader order ------------------------------------------

    [Fact]
    public void Lap_folders_are_ordered_by_start_time_like_the_api()
    {
        using var tmp = new TempDir();
        // RunLapHeader lists Def_009 before Def_008_Def_000 (as in RT22 QR Run_1065 after repeated WinTAX splits).
        var run = FakeRun.Create(tmp, ("Lap_x_Def_009", "1"), ("Lap_x_Def_008_Def_000", "1"));
        SetStart(IOPath.Combine(run, "Lap_x_Def_009"), 1789305300);
        SetStart(IOPath.Combine(run, "Lap_x_Def_008_Def_000"), 1789305200);

        Assert.Equal(["Lap_x_Def_008_Def_000", "Lap_x_Def_009"], LapSplitStore.ReadLapFolders(run).Select(l => l.Folder));
        Assert.Equal(["Lap_x_Def_008_Def_000", "Lap_x_Def_009"], XmlRunEditor.OrderedLapFolders(run).Select(IOPath.GetFileName));

        static void SetStart(string lap, long unixSeconds)
        {
            var file = IOPath.Combine(lap, "LapHeader.xml");
            var doc = XDocument.Load(file);
            doc.Descendants("STS").Single().Value = unixSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture);
            doc.Save(file);
        }
    }

    // ---- B04 / B16 / B18 / B35: .HED handling ----------------------------------------------------------------------

    private const string Hed = """
        [KTM_NAMING]
        disciplineName = MX
        seasonName = 2026
        calendarweek = 37
        teamName = Manual Team
        riderName = Stand In
        riderabbreviationName = AA
        bikeNumber = 01
        bikeName = AA01
        engineClass = 450
        typeName = Racetrack
        countrycode = CHN
        countryName = China
        trackName = Shanghai
        trackLength = 1592
        eventName = RT37_CHN-Shanghai
        sessionName = R1
        runNumber = Run_885
        """;

    [Fact]
    public void ReadMetadata_reads_all_master_data_fields()
    {
        using var tmp = new TempDir();
        var file = tmp.Write("x.HED", Hed);

        var m = new HedFileService().ReadMetadata(file);

        Assert.Equal(("Manual Team", "2026", "MX", "AA", "01"), (m.TeamName, m.SeasonName, m.DisciplineName, m.RiderAbbreviation, m.BikeNumber));
        Assert.Equal(("37", "Racetrack", "CHN", "China"), (m.CalendarWeek, m.TypeName, m.CountryCode, m.CountryName));
        Assert.Equal(string.Empty, m.EventName); // event/session/run always come from the path
    }

    [Fact]
    public void PreferHedMetadata_keeps_a_manual_team_from_the_hed_file()
    {
        using var tmp = new TempDir();
        var hed = new HedFileService().ReadMetadata(tmp.Write("x.HED", Hed));
        var derived = new RunMetadata { RiderName = "Andrea Adamo", TeamName = "KTM Factory Racing", SeasonName = "2026" };

        var differences = MetadataMerger.Merge(hed, derived, preferHed: true);

        Assert.Equal("Manual Team", hed.TeamName);
        Assert.Contains(differences, d => d.Field == "teamName");
    }

    [Fact]
    public void EnsureHedInAllRuns_writes_the_run_number_of_each_run_and_removes_stale_files_after_writing()
    {
        using var tmp = new TempDir();
        var r1 = tmp.CreateDir(IOPath.Combine("AA01", "Run_885"));
        var r2 = tmp.CreateDir(IOPath.Combine("AA01", "Run_886"));
        tmp.Write(IOPath.Combine("AA01", "Run_885", "old name.HED"), Hed);
        var metadata = new RunMetadata { SeasonName = "2026", EventName = "RT37_CHN-Shanghai", BikeName = "AA01", SessionName = "R1", TrackLength = "1593" };

        new HedFileService().EnsureHedInAllRuns([r1, r2], IOPath.GetDirectoryName(r1)!, metadata);

        foreach (var (run, number) in new[] { (r1, "Run_885"), (r2, "Run_886") })
        {
            var files = Directory.GetFiles(run, "*.HED");
            var file = Assert.Single(files);
            Assert.Equal("2026RT37_CHN-Shanghai_AA01R1.HED", IOPath.GetFileName(file));
            var values = new HedFileService().ParseHedFile(file);
            Assert.Equal(number, values["runNumber"]);
            Assert.Equal("1593", values["trackLength"]);
            Assert.Equal("Manual Team", values["teamName"]);
        }
    }

    [Fact]
    public void Surface_is_added_to_an_existing_track_section_instead_of_a_second_one()
    {
        using var tmp = new TempDir();
        var run = tmp.CreateDir("Run_1");
        var file = tmp.Write(IOPath.Combine("Run_1", "a.HED"), Hed + "\n[TRACK]\n");

        new HedFileService().SetTrackSurface(run, "Hardpack");

        var lines = File.ReadAllLines(file);
        Assert.Single(lines, l => l.Trim() == "[TRACK]");
        Assert.Contains("surface = Hardpack", lines);
    }

    // ---- F06: bike name ---------------------------------------------------------------------------------------------

    [Theory]
    [InlineData("AA01", "AA", "01")]
    [InlineData("AA101", "AA", "101")]
    [InlineData("AA1", "AA", "1")]
    [InlineData("JP01", "JP", "01")]
    [InlineData("X", "", "X")]
    [InlineData("", "", "")]
    public void Bike_name_is_split_into_abbreviation_and_number(string bike, string abbreviation, string number) =>
        Assert.Equal((abbreviation, number), KompassService.SplitBikeName(bike));

    // ---- F01: track length text -------------------------------------------------------------------------------------

    [Theory]
    [InlineData("1592", 1592)]
    [InlineData(" 1650 ", 1650)]
    [InlineData("1850,5", 0)]
    [InlineData("1.850", 0)]
    [InlineData("", 0)]
    public void Track_length_text_is_parsed_like_the_track_length_gate(string text, double expected) =>
        Assert.Equal(expected, OraclePipeline.ParseTrackLength(text));

    // ---- B15: race classification with missing channels ------------------------------------------------------------

    [Fact]
    public void Missing_engine_or_throttle_channel_does_not_reject_a_race()
    {
        var thresholds = new RaceDetectionThresholds();

        var noRpm = RaceSegmentDetector.Classify(thresholds, 15000, 50, 40, double.NaN, gpsUsable: true, minDistanceM: 1000);
        var noThrottle = RaceSegmentDetector.Classify(thresholds, 15000, 50, double.NaN, 8000, gpsUsable: true, minDistanceM: 1000);
        var engineOff = RaceSegmentDetector.Classify(thresholds, 15000, 50, 40, 0, gpsUsable: true, minDistanceM: 1000);

        Assert.Equal(RaceRunVerdict.Race, noRpm.Verdict);
        Assert.Equal(RaceRunVerdict.Race, noThrottle.Verdict);
        Assert.Equal(RaceRunVerdict.NoRace, engineOff.Verdict);
        Assert.True(noRpm.LowConfidence);
        Assert.True(noThrottle.LowConfidence);
    }

    [Fact]
    public void Fallback_without_gps_uses_the_available_channel_with_low_confidence()
    {
        var thresholds = new RaceDetectionThresholds();

        var noThrottle = RaceSegmentDetector.Classify(thresholds, 0, 0, double.NaN, 8000, gpsUsable: false, minDistanceM: 1000);
        var noRpm = RaceSegmentDetector.Classify(thresholds, 0, 0, 40, double.NaN, gpsUsable: false, minDistanceM: 1000);
        var noThrottleLowRpm = RaceSegmentDetector.Classify(thresholds, 0, 0, double.NaN, 3000, gpsUsable: false, minDistanceM: 1000);
        var noRpmLowThrottle = RaceSegmentDetector.Classify(thresholds, 0, 0, 10, double.NaN, gpsUsable: false, minDistanceM: 1000);
        var neither = RaceSegmentDetector.Classify(thresholds, 0, 0, double.NaN, double.NaN, gpsUsable: false, minDistanceM: 1000);

        Assert.Equal(RaceRunVerdict.Race, noThrottle.Verdict);
        Assert.True(noThrottle.LowConfidence);
        Assert.Equal(RaceRunVerdict.Race, noRpm.Verdict);
        Assert.True(noRpm.LowConfidence);
        Assert.Equal(RaceRunVerdict.NoRace, noThrottleLowRpm.Verdict);
        Assert.Equal(RaceRunVerdict.NoRace, noRpmLowThrottle.Verdict);
        Assert.Equal(RaceRunVerdict.NoRace, neither.Verdict);
    }

    // ---- B21: invalid configuration ---------------------------------------------------------------------------------

    [Fact]
    public void Invalid_configuration_is_an_error_but_a_missing_one_uses_defaults()
    {
        using var tmp = new TempDir();
        var invalid = tmp.Write("oracle-config.json", "{ \"DatabaseDirectory\": ");
        var nulls = tmp.Write("nulls.json", "{ \"TeamMappings\": null, \"Gps\": null }");

        Assert.Throws<InvalidDataException>(() => OracleConfig.Load(invalid));
        Assert.NotNull(OracleConfig.Load(IOPath.Combine(tmp.Path, "missing.json")));
        var config = OracleConfig.Load(nulls);
        Assert.NotNull(config.TeamMappings);
        Assert.NotNull(config.Gps);
    }

    // ---- F09: status invalidation after split-laps ------------------------------------------------------------------

    [Fact]
    public void Marking_a_processed_folder_outdated_forces_reprocessing()
    {
        using var tmp = new TempDir();
        var status = OracleStatus.Create(tmp.Path);
        status.ExitCode = 0;
        status.Save(tmp.Path);
        Assert.Equal(OracleStatus.State.Done, OracleStatus.GetState(tmp.Path));

        OracleStatus.MarkOutdated(tmp.Path, "laps split");

        Assert.Equal(OracleStatus.State.Outdated, OracleStatus.GetState(tmp.Path));
        OracleStatus.MarkOutdated(IOPath.Combine(tmp.Path, "none"), "no status – no-op");
    }

    // ---- B28: processing lock ---------------------------------------------------------------------------------------

    [Fact]
    public void Lock_on_a_missing_folder_is_an_error_not_a_lock_conflict()
    {
        using var tmp = new TempDir();

        Assert.Throws<DirectoryNotFoundException>(() => ProcessingLock.TryAcquire(IOPath.Combine(tmp.Path, "missing")));
        using var first = ProcessingLock.TryAcquire(tmp.Path);
        Assert.NotNull(first);
        Assert.Null(ProcessingLock.TryAcquire(tmp.Path));
    }

    // ---- B01: write API results -------------------------------------------------------------------------------------

    [Fact]
    public void Write_api_errors_are_reported_with_a_clean_message()
    {
        (uint, uint, uint, string)[] results =
        [
            (0, 0, 0, "No errors.\u0001\u00ff"),
            (33, 0, 33, "Invalid start offset value ( 0 to 999 )\u0002garbage"),
        ];

        var errors = KtmWriteErrors.Describe(results, "lap");

        var error = Assert.Single(errors);
        Assert.Equal("lap 2: Invalid start offset value ( 0 to 999 ) (33 channel(s))", error);
        Assert.Empty(KtmWriteErrors.Describe(null));
        Assert.Equal("unknown error", KtmWriteErrors.Clean("\u0001"));
    }
}
