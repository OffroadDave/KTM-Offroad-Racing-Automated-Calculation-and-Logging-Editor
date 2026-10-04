namespace Ktm.Racing.Oracle.Tests;

public sealed class HedFileServiceTests
{
    [Fact]
    public void ParseHedFile_reads_keys_skips_sections_blank_and_invalid_lines()
    {
        using var tmp = new TempDir();
        var file = tmp.Write("a.HED", string.Join("\n",
            "[KTM_NAMING]",
            "",
            "bikeName = AA01",
            "trackName=Teutschenthal",
            "noEquals",
            "= emptyKey",
            "emptyValue =",
            "[TRACK]",
            "surface = Sand"));

        var values = new HedFileService().ParseHedFile(file);

        Assert.Equal("AA01", values["bikeName"]);
        Assert.Equal("Teutschenthal", values["trackName"]);
        Assert.Equal("Sand", values["surface"]);
        Assert.False(values.ContainsKey("noEquals"));
        Assert.False(values.ContainsKey("emptyValue"));
        Assert.DoesNotContain(values.Keys, k => k.Length == 0);
    }

    [Fact]
    public void ParseHedFile_keys_are_case_insensitive_last_wins()
    {
        using var tmp = new TempDir();
        var file = tmp.Write("a.HED", "bikeName = AA01\nBIKENAME = JP02");
        var values = new HedFileService().ParseHedFile(file);
        Assert.Equal("JP02", values["bikename"]);
    }

    [Fact]
    public void ReadMetadata_normalizes_bikeName_and_reads_surface()
    {
        using var tmp = new TempDir();
        var file = tmp.Write("a.HED", string.Join("\n",
            "bikeName = AA 01",
            "trackName = Teutschenthal",
            "trackLength = 1650",
            "riderName = Test Rider",
            "engineClass = 450",
            "surface = Sand"));

        var m = new HedFileService().ReadMetadata(file);

        Assert.Equal("AA01", m.BikeName);
        Assert.Equal("Teutschenthal", m.TrackName);
        Assert.Equal("1650", m.TrackLength);
        Assert.Equal("Test Rider", m.RiderName);
        Assert.Equal("450", m.EngineClass);
        Assert.Equal("Sand", m.Surface);
    }

    [Fact]
    public void BuildHedFileName_is_canonical_without_spaces()
    {
        var name = HedFileService.BuildHedFileName(new RunMetadata
        {
            SeasonName = "2026",
            EventName = "RT22_GER-Teutschenthal",
            BikeName = "AA01",
            SessionName = "R1",
        });
        Assert.Equal("2026RT22_GER-Teutschenthal_AA01R1.HED", name);
    }

    [Fact]
    public void EnsureHedInAllRuns_writes_canonical_removes_old_normalizes_bikeName_and_appends_surface()
    {
        using var tmp = new TempDir();
        var run1 = tmp.CreateDir("Run_1");
        var run2 = tmp.CreateDir("Run_2");
        File.WriteAllText(Path.Combine(run1, "oldname.HED"),
            "[KTM_NAMING]\nbikeName = AA 01\ntrackName = Teutschenthal\n");

        var metadata = new RunMetadata
        {
            SeasonName = "2026",
            EventName = "RT22_GER-Teutschenthal",
            BikeName = "AA01",
            SessionName = "R1",
            Surface = "Sand",
        };
        var canonical = HedFileService.BuildHedFileName(metadata);

        new HedFileService().EnsureHedInAllRuns(new[] { run1, run2 }, tmp.Path, metadata);

        foreach (var run in new[] { run1, run2 })
        {
            var heds = Directory.GetFiles(run, "*.HED");
            Assert.Single(heds);
            Assert.Equal(canonical, Path.GetFileName(heds[0]));

            var text = File.ReadAllText(heds[0]);
            Assert.Contains("[TRACK]", text);
            Assert.Contains("surface = Sand", text);
            Assert.DoesNotContain("AA 01", text);
        }

        Assert.False(File.Exists(Path.Combine(run1, "oldname.HED")));
    }

    [Fact]
    public void EnsureHedInAllRuns_updates_existing_surface_line()
    {
        using var tmp = new TempDir();
        var run = tmp.CreateDir("Run_1");
        File.WriteAllText(Path.Combine(run, "x.HED"),
            "[KTM_NAMING]\nbikeName = AA01\n[TRACK]\nsurface = Hardpack\n");

        var metadata = new RunMetadata { SeasonName = "2026", EventName = "E", BikeName = "AA01", SessionName = "R1", Surface = "Sand" };
        new HedFileService().EnsureHedInAllRuns(new[] { run }, tmp.Path, metadata);

        var text = File.ReadAllText(Directory.GetFiles(run, "*.HED")[0]);
        Assert.Contains("surface = Sand", text);
        Assert.DoesNotContain("Hardpack", text);
    }

    [Fact]
    public void EnsureHedInAllRuns_does_not_add_track_section_when_surface_empty()
    {
        using var tmp = new TempDir();
        var run = tmp.CreateDir("Run_1");
        File.WriteAllText(Path.Combine(run, "x.HED"), "[KTM_NAMING]\nbikeName = AA01\n");

        var metadata = new RunMetadata { SeasonName = "2026", EventName = "E", BikeName = "AA01", SessionName = "R1", Surface = "" };
        new HedFileService().EnsureHedInAllRuns(new[] { run }, tmp.Path, metadata);

        var text = File.ReadAllText(Directory.GetFiles(run, "*.HED")[0]);
        Assert.DoesNotContain("[TRACK]", text);
    }

    [Fact]
    public void EnsureHedInAllRuns_roundtrips_surface_through_ReadMetadata()
    {
        using var tmp = new TempDir();
        var run = tmp.CreateDir("Run_1");
        File.WriteAllText(Path.Combine(run, "x.HED"), "[KTM_NAMING]\nbikeName = AA01\ntrackName = T\n");

        var metadata = new RunMetadata { SeasonName = "2026", EventName = "E", BikeName = "AA01", SessionName = "R1", Surface = "Loam" };
        var service = new HedFileService();
        service.EnsureHedInAllRuns(new[] { run }, tmp.Path, metadata);

        var reread = service.ReadMetadata(Directory.GetFiles(run, "*.HED")[0]);
        Assert.Equal("Loam", reread.Surface);
    }
}
