using System.Globalization;
using System.Xml.Linq;
using Ktm.Racing.Domain.Geo;

namespace Ktm.Racing.Oracle
{
    /// <summary>Start and end of a lap in the logger clock (from LapHeader.xml).</summary>
    public sealed record LapWindow(string Folder, DateTime Start, DateTime End);

    /// <summary>Weather result of one run.</summary>
    public sealed record RunWeatherResult(string RunPath, string Status, string Message, WeatherSummary? Summary = null, int LapsWritten = 0);

    /// <summary>Result of the weather step for a set of runs (one session/device folder).</summary>
    public sealed record WeatherStepResult(string Status, string Message, IReadOnlyList<RunWeatherResult> Runs, string Source = "")
    {
        /// <summary>Details for <c>oracle.status.json</c>.</summary>
        public Dictionary<string, object> ToDetails()
        {
            var details = new Dictionary<string, object> { ["message"] = Message, ["atUtc"] = DateTime.UtcNow };
            if (Source.Length > 0)
            {
                details["source"] = Source;
                details["attribution"] = "Weather data by Open-Meteo.com (CC BY 4.0)";
            }
            var runs = Runs.Where(r => r.Summary is not null)
                .Select(r => $"{Path.GetFileName(r.RunPath)}: {Describe(r.Summary!)}")
                .ToList();
            if (runs.Count > 0)
                details["runs"] = runs;
            var problems = Runs.Where(r => r.Status is StepStatus.Failed or StepStatus.Skipped)
                .Select(r => $"{Path.GetFileName(r.RunPath)}: {r.Message}")
                .ToList();
            if (problems.Count > 0)
                details["notProcessed"] = problems;
            return details;
        }

        internal static string Describe(WeatherSummary s) => string.Create(CultureInfo.InvariantCulture,
            $"{s.TrackCondition}, {s.AirTempC:0.0} °C ({s.AirTempStartC:0.0}→{s.AirTempEndC:0.0}), {s.HumidityPct:0} %, " +
            $"{s.PressureHpa:0.0} hPa, rain {s.RainMm:0.0} mm (max {s.RainRateMaxMmPerHour:0.0} mm/h), ground {s.GroundTempC:0.0} °C, ρ {s.AirDensityKgM3:0.000} kg/m³");
    }

    /// <summary>
    /// Weather during a session: reads the lap times of the runs (LapHeader.xml), fetches the weather for the session window
    /// once from the <see cref="IWeatherSource"/> and writes per run <c>trackCondition</c> into the <c>[TRACK]</c> section and the
    /// <c>[WEATHER]</c> section of the .HED and per lap
    /// <c>Meteo.xml</c>. Only the time of the runs is evaluated. Runs whose .HED has <c>weatherSource = manual</c> are left
    /// unchanged. Never throws for missing data or network problems – the result says what happened.
    /// </summary>
    public sealed class WeatherService(WeatherSettings settings, IWeatherSource source)
    {
        // Runs further apart than this are fetched separately (e.g. a device folder spanning two days).
        private static readonly TimeSpan MaxGap = TimeSpan.FromHours(6);

        public async Task<WeatherStepResult> ApplyAsync(IReadOnlyList<string> runs, GeoCoordinate? location, CancellationToken cancellationToken = default)
        {
            if (location is null)
                return Skipped(runs, "no track coordinate in KOMPASS – enter latitude/longitude in KODE");

            var results = new List<RunWeatherResult>();
            var timed = new List<(string Run, IReadOnlyList<LapWindow> Laps)>();
            foreach (var run in runs)
            {
                if (string.Equals(HedFileService.ReadRunValue(run, HedFileService.WeatherSection, "weatherSource"),
                        HedFileService.ManualWeatherSource, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(new RunWeatherResult(run, StepStatus.Success, "weather maintained manually – left unchanged"));
                    continue;
                }
                var laps = ReadLapWindows(run);
                if (laps.Count == 0)
                    results.Add(new RunWeatherResult(run, StepStatus.Skipped, "no lap start times in LapHeader.xml"));
                else
                    timed.Add((run, laps));
            }

            var sourceText = "";
            foreach (var cluster in Cluster(timed))
            {
                var from = cluster.Min(r => r.Laps[0].Start);
                var to = cluster.Max(r => r.Laps.Max(l => l.End));
                WeatherSeries series;
                try
                {
                    series = await source.GetAsync(location.Value, from, to, cancellationToken).ConfigureAwait(false);
                }
                catch (WeatherUnavailableException ex)
                {
                    Console.WriteLine($"[weather] ⚠️ {source.Name}: {ex.Message} – retried on the next invocation.");
                    results.AddRange(cluster.Select(r => new RunWeatherResult(r.Run, StepStatus.Failed, ex.Message)));
                    continue;
                }

                var gridKm = WeatherCalculator.DistanceKm(location.Value.Latitude, location.Value.Longitude, series.GridLatitude, series.GridLongitude);
                sourceText = string.Create(CultureInfo.InvariantCulture,
                    $"{source.Name} {series.Model} (grid {gridKm:0.0} km, {(settings.LoggerClockIsUtc ? "UTC" : $"UTC{series.UtcOffsetSeconds / 3600.0:+0.##;-0.##;+0}")})");
                Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
                    $"[weather] {sourceText}: {from:dd.MM.yyyy HH:mm}–{to:HH:mm} logger time, {cluster.Count} run(s)."));

                foreach (var (run, laps) in cluster)
                    results.Add(ApplyToRun(run, laps, series, sourceText));
            }

            return Combine(results, sourceText);
        }

        private RunWeatherResult ApplyToRun(string run, IReadOnlyList<LapWindow> laps, WeatherSeries series, string sourceText)
        {
            var name = Path.GetFileName(Path.TrimEndingDirectorySeparator(run));
            try
            {
                var summary = WeatherCalculator.Summarize(series, laps[0].Start, laps.Max(l => l.End), settings);
                if (double.IsNaN(summary.AirTempC) && double.IsNaN(summary.RainMm))
                    return new RunWeatherResult(run, StepStatus.Failed, "weather model returned no values for the run");

                if (settings.WriteHed)
                    new HedFileService().SetWeather(run, HedValues(summary, sourceText), summary.TrackCondition);

                var written = 0;
                if (settings.WriteMeteoXml)
                {
                    foreach (var lap in laps)
                    {
                        var folder = Path.Combine(run, lap.Folder);
                        if (Directory.Exists(folder) && MeteoXml.Write(folder, WeatherCalculator.Summarize(series, lap.Start, lap.End, settings)))
                            written++;
                    }
                }

                Console.WriteLine($"[weather] {name}: {WeatherStepResult.Describe(summary)}" +
                                  (settings.WriteMeteoXml ? $" – Meteo.xml in {written}/{laps.Count} lap(s)." : "."));
                return new RunWeatherResult(run, StepStatus.Success, "ok", summary, written);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                Console.WriteLine($"[weather] ⚠️ {name}: {ex.Message}");
                return new RunWeatherResult(run, StepStatus.Failed, ex.Message);
            }
        }

        /// <summary>
        /// Key/value pairs of the <c>[WEATHER]</c> section (order and keys as in the MotoDB HED catalog). The track condition is
        /// written separately into <c>[TRACK]</c>.
        /// </summary>
        internal static IReadOnlyList<KeyValuePair<string, string>> HedValues(WeatherSummary s, string sourceText)
        {
            static string F(double value, string format) => double.IsNaN(value) ? "" : value.ToString(format, CultureInfo.InvariantCulture);
            return
            [
                new("airTemp", F(s.AirTempC, "0.0")),
                new("airTempStart", F(s.AirTempStartC, "0.0")),
                new("airTempEnd", F(s.AirTempEndC, "0.0")),
                new("groundTemp", F(s.GroundTempC, "0.0")),
                new("humidity", F(s.HumidityPct, "0")),
                new("pressure", F(s.PressureHpa, "0.0")),
                new("airDensity", F(s.AirDensityKgM3, "0.000")),
                new("windSpeed", F(s.WindSpeedKmh, "0.0")),
                new("windDir", F(s.WindDirectionDeg, "0")),
                new("rain", F(s.RainMm, "0.0")),
                new("rainRateMax", F(s.RainRateMaxMmPerHour, "0.0")),
                new("weatherSource", sourceText),
            ];
        }

        /// <summary>
        /// Laps of a run in data order (<see cref="LapSplitStore.ReadLapFolders"/>) with start (<c>STS</c>+<c>STNs</c>, else
        /// <c>STMs</c>) and end (start + <c>LapTime</c>). Laps without a start time are left out.
        /// </summary>
        public static IReadOnlyList<LapWindow> ReadLapWindows(string runPath)
        {
            var windows = new List<LapWindow>();
            foreach (var lap in LapSplitStore.ReadLapFolders(runPath))
            {
                try
                {
                    var header = Path.Combine(runPath, lap.Folder, "LapHeader.xml");
                    if (!File.Exists(header)) continue;
                    var root = XDocument.Load(header).Root;
                    var info = root?.Element("StartTimeInfo")?.Element("InfoArray")?.Element("StartTimeInfo");
                    DateTime? start = null;
                    if (long.TryParse(info?.Element("STS")?.Value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var seconds))
                    {
                        long.TryParse(info?.Element("STNs")?.Value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var nanos);
                        start = DateTime.SpecifyKind(DateTime.UnixEpoch.AddSeconds(seconds).AddTicks(nanos / 100), DateTimeKind.Unspecified);
                    }
                    else if (DateTime.TryParseExact(info?.Element("STMs")?.Value.Trim(), "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture,
                                 DateTimeStyles.None, out var wall))
                    {
                        start = wall;
                    }
                    if (start is null) continue;

                    double.TryParse(root?.Element("LapTime")?.Value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var lapTime);
                    var duration = double.IsFinite(lapTime) && lapTime > 0 ? TimeSpan.FromSeconds(lapTime) : TimeSpan.Zero;
                    windows.Add(new LapWindow(lap.Folder, start.Value, start.Value + duration));
                }
                catch (Exception ex) when (ex is IOException or System.Xml.XmlException or UnauthorizedAccessException)
                {
                    // A lap without a readable header gets no Meteo.xml; the other laps are still processed.
                }
            }
            return windows.OrderBy(w => w.Start).ToList();
        }

        private static IEnumerable<List<(string Run, IReadOnlyList<LapWindow> Laps)>> Cluster(List<(string Run, IReadOnlyList<LapWindow> Laps)> runs)
        {
            List<(string Run, IReadOnlyList<LapWindow> Laps)>? current = null;
            var currentEnd = DateTime.MinValue;
            foreach (var run in runs.OrderBy(r => r.Laps[0].Start))
            {
                if (current is null || run.Laps[0].Start - currentEnd > MaxGap)
                {
                    if (current is not null) yield return current;
                    current = [];
                    currentEnd = DateTime.MinValue;
                }
                current.Add(run);
                var end = run.Laps.Max(l => l.End);
                if (end > currentEnd) currentEnd = end;
            }
            if (current is not null) yield return current;
        }

        private static WeatherStepResult Skipped(IReadOnlyList<string> runs, string reason)
        {
            Console.WriteLine($"[weather] Skipped: {reason}.");
            return new WeatherStepResult(StepStatus.Skipped, reason, runs.Select(r => new RunWeatherResult(r, StepStatus.Skipped, reason)).ToList());
        }

        private static WeatherStepResult Combine(List<RunWeatherResult> results, string source)
        {
            var ok = results.Count(r => r.Status == StepStatus.Success);
            var failed = results.Count(r => r.Status == StepStatus.Failed);
            var status = failed == 0 ? (ok > 0 ? StepStatus.Success : StepStatus.Skipped)
                : ok > 0 ? StepStatus.Partial
                : StepStatus.Failed;
            var message = $"{ok} run(s) with weather, {failed} failed, {results.Count - ok - failed} skipped";
            return new WeatherStepResult(status, message, results, source);
        }
    }
}
