namespace Ktm.Racing.Oracle.Tests;

public sealed class OracleConfigTests
{
    [Fact]
    public void Default_team_mappings_are_case_insensitive()
    {
        var config = new OracleConfig();
        Assert.Equal("KTM Factory Racing", config.TeamMappings["01_KTM-FR"]);
        Assert.Equal("KTM Factory Racing", config.TeamMappings["ktm-fr"]);
        Assert.Equal("Husqvarna Factory Racing", config.TeamMappings["HQV"]);
    }

    [Fact]
    public void DatabaseFilePath_combines_directory_and_file()
    {
        var config = new OracleConfig { DatabaseDirectory = @"C:\x\y", DatabaseFileName = "k.json" };
        Assert.Equal(Path.Combine(@"C:\x\y", "k.json"), config.DatabaseFilePath);
    }

    [Fact]
    public void GpsLapSplit_defaults_match_domain_defaults()
    {
        var options = new GpsLapSplitSettings().ToOptions();
        Assert.Equal(25.0, options.RadiusMeters);
        Assert.Equal(20.0, options.MinLapSeconds);
        Assert.True(options.IncludeTrailingPartialLap);
    }

    [Fact]
    public void GpsLapSplit_ToOptions_maps_all_values()
    {
        var settings = new GpsLapSplitSettings { RadiusMeters = 42, MinLapSeconds = 7, IncludeTrailingPartialLap = false };
        var options = settings.ToOptions();
        Assert.Equal(42.0, options.RadiusMeters);
        Assert.Equal(7.0, options.MinLapSeconds);
        Assert.False(options.IncludeTrailingPartialLap);
    }

    [Fact]
    public void Load_never_throws_and_returns_usable_config()
    {
        var config = OracleConfig.Load();
        Assert.NotNull(config);
        Assert.NotEmpty(config.TeamMappings);
        Assert.NotNull(config.RaceDetection);
        Assert.NotNull(config.GpsLapSplit);
    }
}
