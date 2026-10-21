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
        var settings = new GpsLapSplitSettings();
        var options = settings.ToOptions();
        var domain = new Ktm.Racing.Domain.Telemetry.GpsLapSplitOptions();
        Assert.Equal(domain.MinLapSeconds, options.MinLapSeconds);
        Assert.Equal(domain.MaxSampleGapSeconds, options.MaxSampleGapSeconds);
        Assert.Equal(domain.MinCrossingAngleDegrees, options.MinCrossingAngleDegrees);
        Assert.Equal(domain.MinSpeedKph, options.MinSpeedKph);
        Assert.Equal(new Ktm.Racing.Domain.Telemetry.FinishLineBuildOptions().SnapRadiusMeters, settings.ToLineOptions().SnapRadiusMeters);
        Assert.Equal(new Ktm.Racing.Domain.Telemetry.LapSplitDetectorOptions().BoundaryDistanceMeters, settings.ToDetectorOptions().BoundaryDistanceMeters);
    }

    [Fact]
    public void Gps_settings_default_to_Lat_dez_and_map_to_the_cleaner_and_splitter()
    {
        var gps = new GpsSettings { MaxSpeedKph = 150, MinJumpMeters = 8, MinSegmentSeconds = 3 };
        var clean = gps.ToCleanOptions();
        var split = new GpsLapSplitSettings().ToOptions(gps);

        Assert.Equal(("Lat_dez", "Lon_dez"), (new GpsSettings().LatitudeChannel, new GpsSettings().LongitudeChannel));
        Assert.Equal((150.0, 8.0, 3.0), (clean.MaxSpeedKph, clean.MinJumpMeters, clean.MinSegmentSeconds));
        Assert.Equal((150.0, 8.0), (split.MaxSpeedKph, split.MinJumpMeters));
        Assert.Equal(new Ktm.Racing.Domain.Telemetry.GpsCleanOptions().MaxSpeedKph, new GpsSettings().ToCleanOptions().MaxSpeedKph);
        Assert.NotNull(OracleConfig.Load().Gps);
    }

    [Fact]
    public void Gps_channels_are_resolved_exactly_never_by_substring()
    {
        string[] names = ["Lat_Valid", "aLat", "LAT_DEZ", "Lon_dez"];

        Assert.Equal("LAT_DEZ", GpsSettings.Resolve(names, "Lat_dez"));
        Assert.Equal("Lon_dez", GpsSettings.Resolve(names, "Lon_dez"));
        Assert.Null(GpsSettings.Resolve(["Lat_Valid", "aLat"], "Lat_dez"));
    }

    [Fact]
    public void GpsLapSplit_settings_map_to_all_option_sets()
    {
        var settings = new GpsLapSplitSettings
        {
            MinLapSeconds = 7, MaxSampleGapSeconds = 2, MinCrossingAngleDegrees = 45, MinSpeedKph = 9,
            SnapRadiusMeters = 80, LineHalfWidthMeters = 20, BoundaryToleranceSeconds = 3, BoundaryDistanceMeters = 25,
        };
        var options = settings.ToOptions();
        var line = settings.ToLineOptions();
        var detector = settings.ToDetectorOptions();
        Assert.Equal((7.0, 2.0, 45.0, 9.0), (options.MinLapSeconds, options.MaxSampleGapSeconds, options.MinCrossingAngleDegrees, options.MinSpeedKph));
        Assert.Equal((80.0, 20.0), (line.SnapRadiusMeters, line.HalfWidthMeters));
        Assert.Equal((3.0, 25.0), (detector.BoundaryToleranceSeconds, detector.BoundaryDistanceMeters));
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
