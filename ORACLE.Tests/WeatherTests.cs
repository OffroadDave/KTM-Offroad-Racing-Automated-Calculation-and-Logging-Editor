using System.Globalization;
using System.Xml.Linq;
using Ktm.Racing.Domain.Geo;
using Ktm.Racing.Domain.MotoDb;

namespace Ktm.Racing.Oracle.Tests;

public sealed class WeatherTests
{
    private static readonly DateTime Noon = new(2026, 5, 31, 12, 0, 0);

    /// <summary>15-minute series 12:00–13:00: temperature +1 °C per step, constant humidity/pressure, rain in two intervals.</summary>
    private static WeatherSeries Series(double[]? rain = null, double[]? direction = null) => new()
    {
        Time = Enumerable.Range(0, 5).Select(i => Noon.AddMinutes(15 * i)).ToArray(),
        TemperatureC = [20, 21, 22, 23, 24],
        HumidityPct = [70, 70, 70, 70, 70],
        PrecipitationMm = rain ?? [0, 0.4, 0.8, 0, 0],
        WindSpeedKmh = [10, 10, 10, 10, 10],
        WindDirectionDeg = direction ?? [270, 270, 270, 270, 270],
        PressureHpa = [998, 998, 998, 998, 998],
        HourlyTime = [Noon, Noon.AddHours(1)],
        GroundTemperatureC = [22, 26],
        GridLatitude = 51.46,
        GridLongitude = 11.8,
        UtcOffsetSeconds = 7200,
        Model = "best_match",
    };

    // ---- calculations ------------------------------------------------------------------------------------------------

    [Fact]
    public void Values_are_interpolated_and_averaged_over_the_window()
    {
        var s = Series();
        Assert.Equal(20.5, WeatherCalculator.ValueAt(s.Time, s.TemperatureC, Noon.AddMinutes(7.5)), 6);
        Assert.Equal(21.5, WeatherCalculator.Mean(s.Time, s.TemperatureC, Noon.AddMinutes(15), Noon.AddMinutes(30)), 6);
        Assert.Equal(22.0, WeatherCalculator.Mean(s.Time, s.TemperatureC, Noon, Noon.AddMinutes(60)), 6);
        Assert.True(double.IsNaN(WeatherCalculator.ValueAt(s.Time, s.TemperatureC, Noon.AddHours(3))));
    }

    [Fact]
    public void Rain_is_counted_proportionally_to_the_overlap_of_each_interval()
    {
        var s = Series();
        // (12:00,12:15] = 0.4 mm → 10 of 15 min; (12:15,12:30] = 0.8 mm → 5 of 15 min.
        var rain = WeatherCalculator.RainSum(s.Time, s.PrecipitationMm, s.Step, Noon.AddMinutes(5), Noon.AddMinutes(20));
        Assert.Equal(0.4 * 10 / 15 + 0.8 * 5 / 15, rain, 6);
        Assert.Equal(3.2, WeatherCalculator.MaxRate(s.Time, s.PrecipitationMm, s.Step, Noon.AddMinutes(5), Noon.AddMinutes(20)), 6);
        Assert.Equal(0, WeatherCalculator.RainSum(s.Time, s.PrecipitationMm, s.Step, Noon.AddMinutes(31), Noon.AddMinutes(59)), 6);
    }

    [Theory]
    [InlineData(0.0, 0.0, "dry")]
    [InlineData(0.05, 0.2, "dry")]
    [InlineData(0.2, 0.8, "damp")]
    [InlineData(1.2, 1.5, "wet")]
    [InlineData(0.5, 2.4, "wet")]
    [InlineData(double.NaN, double.NaN, "")]
    public void Track_condition_follows_rain_during_the_run(double rainMm, double rate, string expected) =>
        Assert.Equal(expected, WeatherCalculator.Classify(rainMm, rate, new WeatherSettings()));

    [Fact]
    public void Air_density_matches_reference_values()
    {
        // ISA sea level, dry: 1.225 kg/m³.
        Assert.Equal(1.225, WeatherCalculator.AirDensity(15, 1013.25, 0), 3);
        // RT22 R1 (21.6 °C, 998.7 hPa from the model, 71 %): ρ ≈ 1.172 kg/m³.
        Assert.Equal(1.172, WeatherCalculator.AirDensity(21.6, 998.7, 71), 3);
    }

    [Fact]
    public void Wind_direction_is_averaged_as_a_vector()
    {
        var s = Series(direction: [350, 10, 350, 10, 350]);
        var mean = WeatherCalculator.MeanDirection(s.Time, s.WindDirectionDeg, Noon, Noon.AddMinutes(60));
        Assert.True(mean < 5 || mean > 355, $"mean {mean}");
    }

    [Fact]
    public void Summary_covers_the_run_window()
    {
        var summary = WeatherCalculator.Summarize(Series(), Noon.AddMinutes(15), Noon.AddMinutes(45), new WeatherSettings());

        Assert.Equal(22.0, summary.AirTempC, 6);
        Assert.Equal(21.0, summary.AirTempStartC, 6);
        Assert.Equal(23.0, summary.AirTempEndC, 6);
        Assert.Equal(0.8, summary.RainMm, 6);
        Assert.Equal("wet", summary.TrackCondition); // 0.8 mm in 15 min = 3.2 mm/h ≥ WetRateMmPerHour
        Assert.Equal(24.0, summary.GroundTempC, 6);
        Assert.Equal(270, summary.WindDirectionDeg, 6);
    }

    // ---- Open-Meteo -------------------------------------------------------------------------------------------------

    private const string SampleResponse = """
        {"latitude":51.448185,"longitude":11.796082,"utc_offset_seconds":7200,"elevation":115.0,
         "minutely_15":{"time":["2026-05-31T14:15","2026-05-31T14:30"],"temperature_2m":[21.1,null],
           "relative_humidity_2m":[72,72],"precipitation":[0.00,0.10],"wind_speed_10m":[10.4,9.5],
           "wind_direction_10m":[272,281],"surface_pressure":[998.5,998.4]},
         "hourly":{"time":["2026-05-31T14:00"],"soil_temperature_0cm":[24.5]}}
        """;

    [Fact]
    public void Response_is_parsed_with_missing_values_as_NaN()
    {
        var s = OpenMeteoWeatherSource.Parse(SampleResponse, "best_match");

        Assert.Equal(new DateTime(2026, 5, 31, 14, 15, 0), s.Time[0]);
        Assert.Equal(TimeSpan.FromMinutes(15), s.Step);
        Assert.Equal(21.1, s.TemperatureC[0]);
        Assert.True(double.IsNaN(s.TemperatureC[1]));
        Assert.Equal(0.1, s.PrecipitationMm[1]);
        Assert.Equal(998.4, s.PressureHpa[1]);
        Assert.Equal(24.5, s.GroundTemperatureC[0]);
        Assert.Equal(7200, s.UtcOffsetSeconds);
        Assert.Equal(51.448185, s.GridLatitude, 6);
    }

    [Theory]
    [InlineData("""{"error":true,"reason":"Parameter 'start_minutely_15' is out of allowed range"}""", "out of allowed range")]
    [InlineData("""{"latitude":1}""", "without 15-minute values")]
    [InlineData("not json", "invalid response")]
    public void Error_responses_raise_WeatherUnavailable(string json, string expected)
    {
        var ex = Assert.Throws<WeatherUnavailableException>(() => OpenMeteoWeatherSource.Parse(json, "best_match"));
        Assert.Contains(expected, ex.Message);
    }

    [Fact]
    public void Request_covers_the_session_window_in_local_track_time()
    {
        var url = OpenMeteoWeatherSource.BuildUrl(new WeatherSettings(), new GeoCoordinate(51.45880, 11.79560),
            new DateTime(2026, 5, 31, 14, 23, 50), new DateTime(2026, 5, 31, 14, 53, 2), apiKey: null);

        Assert.StartsWith("https://historical-forecast-api.open-meteo.com/v1/forecast?latitude=51.4588&longitude=11.7956", url);
        Assert.Contains("start_minutely_15=2026-05-31T14%3A15", url);
        Assert.Contains("end_minutely_15=2026-05-31T15%3A15", url); // interval ending after the run end is included
        Assert.Contains("start_hour=2026-05-31T14%3A00", url);
        Assert.Contains("end_hour=2026-05-31T15%3A00", url);
        Assert.Contains("timezone=auto", url);
        Assert.DoesNotContain("models=", url);
        Assert.DoesNotContain("apikey", url);
    }

    [Fact]
    public void Request_uses_UTC_model_and_api_key_when_configured()
    {
        var settings = new WeatherSettings { LoggerClock = "Utc", Model = "icon_d2" };
        var url = OpenMeteoWeatherSource.BuildUrl(settings, new GeoCoordinate(1, 2), Noon, Noon.AddMinutes(10), "secret");

        Assert.Contains("timezone=GMT", url);
        Assert.Contains("models=icon_d2", url);
        Assert.Contains("apikey=secret", url);
    }

    // ---- Meteo.xml --------------------------------------------------------------------------------------------------

    private static WeatherSummary Lap() => new()
    {
        AirTempC = 21.64, HumidityPct = 71.4, PressureHpa = 998.53, WindSpeedKmh = 9.36, WindDirectionDeg = 281, RainMm = 0.026,
    };

    [Fact]
    public void Meteo_xml_has_the_WinTAX_layout()
    {
        var xml = MeteoXml.Build(Lap());

        Assert.Equal(
            "<?xml version=\"1.0\"?>\r\n<Meteo>\r\n<TempIn>21.6</TempIn>\r\n<TempOut>21.6</TempOut>\r\n<WindSp>9.4</WindSp>\r\n" +
            "<WindDir>-79</WindDir>\r\n<Bar>998.5</Bar>\r\n<HumIn>71</HumIn>\r\n<HumOut>71</HumOut>\r\n<Rain>0.03</Rain>\r\n</Meteo>\r\n", xml);
        var root = XDocument.Parse(xml!).Root!;
        Assert.Equal(["TempIn", "TempOut", "WindSp", "WindDir", "Bar", "HumIn", "HumOut", "Rain"], root.Elements().Select(e => e.Name.LocalName));
    }

    [Fact]
    public void Meteo_xml_writes_nothing_for_unknown_values()
    {
        Assert.Null(MeteoXml.Build(Lap() with { AirTempC = double.NaN }));
        Assert.Null(MeteoXml.Build(Lap() with { RainMm = double.NaN }));
    }

    [Fact]
    public void Meteo_xml_is_culture_invariant()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            Assert.Contains("<TempOut>21.6</TempOut>", MeteoXml.Build(Lap()));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    // ---- .HED [WEATHER] ---------------------------------------------------------------------------------------------

    [Fact]
    public void Weather_section_is_replaced_and_read_back()
    {
        string[] lines = ["[KTM_NAMING]", "bikeName = AA01", "[WEATHER]", "airTemp = 10.0", "[TRACK]", "surface = Sand", ""];

        var updated = HedFileService.ReplaceSection(lines, "WEATHER", [new("airTemp", "21.6"), new("rain", "0.0")]);

        Assert.Equal(["[KTM_NAMING]", "bikeName = AA01", "[TRACK]", "surface = Sand", "[WEATHER]", "airTemp = 21.6", "rain = 0.0"], updated);
        Assert.Equal("21.6", HedFileService.ReadValue(updated, "WEATHER", "airTemp"));
        Assert.Null(HedFileService.ReadValue(updated, "WEATHER", "surface"));
        Assert.Equal(["[KTM_NAMING]", "bikeName = AA01", "[TRACK]", "surface = Sand"], HedFileService.RemoveSection(updated, "WEATHER"));
    }

    [Fact]
    public void Track_condition_is_set_in_the_TRACK_section()
    {
        string[] withTrack = ["[KTM_NAMING]", "bikeName = AA01", "[TRACK]", "surface = Sand", "", "[WEATHER]", "airTemp = 21.6"];
        var updated = HedFileService.SetSectionValue(withTrack, "TRACK", "trackCondition", "wet");
        Assert.Equal(["[KTM_NAMING]", "bikeName = AA01", "[TRACK]", "surface = Sand", "trackCondition = wet", "", "[WEATHER]", "airTemp = 21.6"], updated);
        Assert.Equal("dry", HedFileService.ReadValue(HedFileService.SetSectionValue(updated, "TRACK", "trackCondition", "dry"), "TRACK", "trackCondition"));

        string[] withoutTrack = ["[KTM_NAMING]", "bikeName = AA01", "[WEATHER]", "airTemp = 21.6"];
        Assert.Equal(["[KTM_NAMING]", "bikeName = AA01", "[TRACK]", "trackCondition = damp", "[WEATHER]", "airTemp = 21.6"],
            HedFileService.SetSectionValue(withoutTrack, "TRACK", "trackCondition", "damp"));

        Assert.Equal(["[KTM_NAMING]", "bikeName = AA01", "[TRACK]", "surface = Sand", "", "[WEATHER]", "airTemp = 21.6"],
            HedFileService.RemoveKey(updated, "TRACK", "trackCondition"));
    }

    [Fact]
    public void Rewriting_the_HED_keeps_each_runs_own_weather_and_never_copies_it()
    {
        using var tmp = new TempDir();
        var metadata = new RunMetadata
        {
            BikeName = "AA01", TrackName = "Teutschenthal", SeasonName = "2026", EventName = "RT22_GER-Teutschenthal", SessionName = "R1",
            Surface = "Sand",
        };
        var name = HedFileService.BuildHedFileName(metadata);
        var run1 = tmp.CreateDir("Run_1");
        var run2 = tmp.CreateDir("Run_2");
        File.WriteAllLines(Path.Combine(run1, name),
            ["[KTM_NAMING]", "bikeName = AA01", "[TRACK]", "surface = Sand", "trackCondition = wet", "[WEATHER]", "airTemp = 21.6"]);

        new HedFileService().EnsureHedInAllRuns([run1, run2], tmp.Path, metadata);

        var hed1 = File.ReadAllLines(Path.Combine(run1, name));
        var hed2 = File.ReadAllLines(Path.Combine(run2, name));
        Assert.Equal("21.6", HedFileService.ReadValue(hed1, "WEATHER", "airTemp"));
        Assert.Equal("wet", HedFileService.ReadValue(hed1, "TRACK", "trackCondition"));
        Assert.Equal("Sand", HedFileService.ReadValue(hed1, "TRACK", "surface"));
        Assert.Single(hed1, l => l.Trim() == "[WEATHER]");
        Assert.Single(hed1, l => l.Trim() == "[TRACK]");
        Assert.DoesNotContain(hed2, l => l.Trim() == "[WEATHER]");
        Assert.Null(HedFileService.ReadValue(hed2, "TRACK", "trackCondition"));
        Assert.Equal("Sand", HedFileService.ReadValue(hed2, "TRACK", "surface"));
        Assert.Equal("Run_2", new HedFileService().ParseHedFile(Path.Combine(run2, name))["runNumber"]);
    }

    [Fact]
    public void Weather_fields_are_part_of_the_MotoDB_HED_catalog_in_file_order()
    {
        var keys = WeatherService.HedValues(new WeatherSummary(), "x").Select(v => v.Key).ToList();
        Assert.Equal(HedFieldCatalog.WeatherFields.Select(f => f.Name), keys);
        Assert.All(HedFieldCatalog.WeatherFields, f => Assert.Contains(f, HedFieldCatalog.Fields));
        Assert.DoesNotContain("trackCondition", keys);
        Assert.DoesNotContain("powerCorrection", keys);
        var track = HedFieldCatalog.Fields.Single(f => f.Name == HedFileService.TrackConditionKey);
        Assert.Equal(HedFieldCatalog.TrackGroup, track.Group);
    }

    // ---- lap windows and service ------------------------------------------------------------------------------------

    /// <summary>Run folder with laps starting at the given local times (logger clock) and the given lap times.</summary>
    private static string CreateRun(TempDir tmp, string name, DateTime start, params double[] lapTimes)
    {
        var run = tmp.CreateDir(name);
        var infos = new List<XElement>();
        var t = start;
        for (var i = 0; i < lapTimes.Length; i++)
        {
            var lap = $"Lap_0000{i + 1}_00418";
            Directory.CreateDirectory(Path.Combine(run, lap));
            File.WriteAllBytes(Path.Combine(run, lap, "cableData.ztx"), new byte[16]);
            var seconds = (long)(t - DateTime.UnixEpoch).TotalSeconds;
            new XDocument(new XElement("LapHeader",
                new XElement("LapTime", lapTimes[i].ToString(CultureInfo.InvariantCulture)),
                new XElement("StartTimeInfo", new XElement("InfoArray", new XElement("StartTimeInfo",
                    new XElement("STMs", t.ToString("dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)),
                    new XElement("STS", seconds),
                    new XElement("STNs", (t.Ticks % TimeSpan.TicksPerSecond) * 100))))))
                .Save(Path.Combine(run, lap, "LapHeader.xml"));
            infos.Add(new XElement("Lap", new XElement("LapInfo", new XElement("Path", $"{lap}\\cableData.ztx"))));
            t = t.AddSeconds(lapTimes[i]);
        }
        new XDocument(new XElement("RunLapHeader", new XElement("Run", new XElement("LapArray", infos)))).Save(Path.Combine(run, "RunLapHeader.xml"));
        File.WriteAllLines(Path.Combine(run, "2026RT22_AA01R1.HED"), ["[KTM_NAMING]", "bikeName = AA01"]);
        return run;
    }

    [Fact]
    public void Lap_windows_come_from_the_lap_headers()
    {
        using var tmp = new TempDir();
        var run = CreateRun(tmp, "Run_1", Noon.AddMinutes(10).AddMilliseconds(141), 48.179, 120);

        var laps = WeatherService.ReadLapWindows(run);

        Assert.Equal(2, laps.Count);
        Assert.Equal(Noon.AddMinutes(10).AddMilliseconds(141), laps[0].Start);
        Assert.Equal(laps[0].Start.AddSeconds(48.179), laps[0].End);
        Assert.Equal(laps[0].End, laps[1].Start);
    }

    private sealed class FakeSource(WeatherSeries? series, Exception? error = null) : IWeatherSource
    {
        public int Calls { get; private set; }
        public (DateTime From, DateTime To) LastWindow { get; private set; }
        public string Name => "Fake";

        public Task<WeatherSeries> GetAsync(GeoCoordinate location, DateTime from, DateTime to, CancellationToken cancellationToken = default)
        {
            Calls++;
            LastWindow = (from, to);
            return error is null ? Task.FromResult(series!) : Task.FromException<WeatherSeries>(error);
        }
    }

    [Fact]
    public async Task Service_writes_HED_and_Meteo_xml_for_all_runs_of_the_session_with_one_request()
    {
        using var tmp = new TempDir();
        var run1 = CreateRun(tmp, "Run_1", Noon.AddMinutes(5), 300, 300);
        var run2 = CreateRun(tmp, "Run_2", Noon.AddMinutes(30), 600);
        var source = new FakeSource(Series());

        var result = await new WeatherService(new WeatherSettings(), source).ApplyAsync([run1, run2], new GeoCoordinate(51.4588, 11.7956));

        Assert.Equal(StepStatus.Success, result.Status);
        Assert.Equal(1, source.Calls);
        Assert.Equal((Noon.AddMinutes(5), Noon.AddMinutes(40)), source.LastWindow);

        var hed1 = File.ReadAllLines(Path.Combine(run1, "2026RT22_AA01R1.HED"));
        // Run 1 = 12:05–12:15: 0.4 mm × 10/15 = 0.27 mm → damp; mean temperature (20.33 + 21.0) / 2.
        Assert.Equal("damp", HedFileService.ReadValue(hed1, "TRACK", "trackCondition"));
        Assert.Null(HedFileService.ReadValue(hed1, "WEATHER", "trackCondition"));
        Assert.Null(HedFileService.ReadValue(hed1, "WEATHER", "powerCorrection"));
        Assert.Equal("0.3", HedFileService.ReadValue(hed1, "WEATHER", "rain"));
        Assert.Equal("20.7", HedFileService.ReadValue(hed1, "WEATHER", "airTemp"));
        Assert.StartsWith("Fake best_match (grid", HedFileService.ReadValue(hed1, "WEATHER", "weatherSource"));
        Assert.Equal(["[KTM_NAMING]", "[TRACK]", "[WEATHER]"], hed1.Where(l => l.StartsWith('[')));

        // Run 2 = 12:30–12:40, dry.
        var hed2 = File.ReadAllLines(Path.Combine(run2, "2026RT22_AA01R1.HED"));
        Assert.Equal("22.7", HedFileService.ReadValue(hed2, "WEATHER", "airTempEnd"));
        Assert.Equal("dry", HedFileService.ReadValue(hed2, "TRACK", "trackCondition"));

        Assert.True(File.Exists(Path.Combine(run1, "Lap_00001_00418", "Meteo.xml")));
        Assert.True(File.Exists(Path.Combine(run1, "Lap_00002_00418", "Meteo.xml")));
        var lap2 = XDocument.Load(Path.Combine(run1, "Lap_00002_00418", "Meteo.xml")).Root!;
        Assert.Equal("20.8", lap2.Element("TempOut")!.Value); // lap 2 = 12:10–12:15
        Assert.Equal("0.13", lap2.Element("Rain")!.Value);
        Assert.Equal(2, result.Runs.Single(r => r.RunPath == run1).LapsWritten);
        Assert.Contains("runs", result.ToDetails().Keys);
    }

    [Fact]
    public async Task Manual_weather_is_never_overwritten()
    {
        using var tmp = new TempDir();
        var run = CreateRun(tmp, "Run_1", Noon.AddMinutes(5), 300);
        var hed = Path.Combine(run, "2026RT22_AA01R1.HED");
        File.WriteAllLines(hed, ["[KTM_NAMING]", "bikeName = AA01", "[TRACK]", "trackCondition = wet", "[WEATHER]", "weatherSource = manual"]);
        var source = new FakeSource(Series());

        var result = await new WeatherService(new WeatherSettings(), source).ApplyAsync([run], new GeoCoordinate(1, 2));

        Assert.Equal(StepStatus.Success, result.Status);
        Assert.Equal(0, source.Calls);
        Assert.Equal("wet", HedFileService.ReadValue(File.ReadAllLines(hed), "TRACK", "trackCondition"));
        Assert.False(File.Exists(Path.Combine(run, "Lap_00001_00418", "Meteo.xml")));
    }

    [Fact]
    public async Task Unavailable_source_or_missing_coordinate_fail_softly()
    {
        using var tmp = new TempDir();
        var run = CreateRun(tmp, "Run_1", Noon.AddMinutes(5), 300);

        var offline = await new WeatherService(new WeatherSettings(), new FakeSource(null, new WeatherUnavailableException("no connection")))
            .ApplyAsync([run], new GeoCoordinate(1, 2));
        Assert.Equal(StepStatus.Failed, offline.Status);
        Assert.Contains("no connection", offline.Runs.Single().Message);
        Assert.DoesNotContain("[WEATHER]", File.ReadAllText(Path.Combine(run, "2026RT22_AA01R1.HED")));

        var noCoordinate = await new WeatherService(new WeatherSettings(), new FakeSource(Series())).ApplyAsync([run], null);
        Assert.Equal(StepStatus.Skipped, noCoordinate.Status);
    }

    [Fact]
    public async Task Runs_hours_apart_are_fetched_separately()
    {
        using var tmp = new TempDir();
        var morning = CreateRun(tmp, "Run_1", Noon.AddHours(-8), 300);
        var noon = CreateRun(tmp, "Run_2", Noon.AddMinutes(5), 300);
        var source = new FakeSource(Series());

        await new WeatherService(new WeatherSettings(), source).ApplyAsync([morning, noon], new GeoCoordinate(1, 2));

        Assert.Equal(2, source.Calls);
    }

    // ---- command line / configuration -------------------------------------------------------------------------------

    [Fact]
    public void Weather_command_accepts_force_only()
    {
        Assert.Null(CommandLine.Validate(["weather", @"C:\RT22", "--force"]));
        Assert.Equal("Unknown option: --dry-run", CommandLine.Validate(["weather", @"C:\RT22", "--dry-run"]));
    }

    [Fact]
    public void Shipped_configuration_contains_the_weather_section()
    {
        var shipped = Path.Combine(AppContext.BaseDirectory, "oracle-config.json");
        var config = OracleConfig.Load(shipped);

        Assert.True(config.Weather.Enabled);
        Assert.Equal("TrackLocal", config.Weather.LoggerClock);
        Assert.Equal("https://historical-forecast-api.open-meteo.com/v1/forecast", config.Weather.BaseUrl);
        Assert.Equal(0.1, config.Weather.DampRainMm);
        Assert.Equal(1.0, config.Weather.WetRainMm);
        Assert.Equal(2.0, config.Weather.WetRateMmPerHour);
    }
}
