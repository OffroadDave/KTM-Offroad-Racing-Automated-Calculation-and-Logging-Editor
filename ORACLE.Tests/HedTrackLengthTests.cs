namespace Ktm.Racing.Oracle.Tests;

public sealed class HedTrackLengthTests
{
    [Fact]
    public void Existing_trackLength_line_is_replaced()
    {
        string[] lines = ["[KTM_NAMING]", "trackName = Lommel", "trackLength = 1000", "[TRACK]", "surface = Sand"];
        var result = HedFileService.EnsureTrackLengthLine(lines, "1851");
        Assert.Equal("trackLength = 1851", result[2]);
        Assert.Equal(5, result.Length);
    }

    [Fact]
    public void Missing_line_is_inserted_after_trackName()
    {
        string[] lines = ["[KTM_NAMING]", "trackName = Lommel", "riderName = X", "[TRACK]", "surface = Sand"];
        var result = HedFileService.EnsureTrackLengthLine(lines, "1851");
        Assert.Equal(["[KTM_NAMING]", "trackName = Lommel", "trackLength = 1851", "riderName = X", "[TRACK]", "surface = Sand"], result);
    }

    [Fact]
    public void Without_trackName_the_line_ends_the_first_section()
    {
        string[] lines = ["[KTM_NAMING]", "riderName = X", "[TRACK]", "surface = Sand"];
        var result = HedFileService.EnsureTrackLengthLine(lines, "1851");
        Assert.Equal(["[KTM_NAMING]", "riderName = X", "trackLength = 1851", "[TRACK]", "surface = Sand"], result);
    }

    [Fact]
    public void Empty_length_keeps_the_content()
    {
        string[] lines = ["[KTM_NAMING]", "trackLength = 1000"];
        Assert.Equal(lines, HedFileService.EnsureTrackLengthLine([.. lines], " "));
    }

    [Fact]
    public void SetTrackLength_updates_all_hed_files_of_a_run()
    {
        using var tmp = new TempDir();
        var run = tmp.CreateDir("Run_1");
        var hed = tmp.Write(Path.Combine("Run_1", "2026MXGP_AA01R1.HED"), "[KTM_NAMING]\ntrackName = Lommel\ntrackLength = 1000\n");

        new HedFileService().SetTrackLength(run, "1851");

        Assert.Equal("1851", new HedFileService().ParseHedFile(hed)["trackLength"]);
    }
}
