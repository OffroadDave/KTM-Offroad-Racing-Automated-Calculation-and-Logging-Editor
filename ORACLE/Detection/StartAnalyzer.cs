using KtmDataHandler.Marelli.Wintax;
using KtmDataHandler.Models.Channels;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Liest die für die Start-Erkennung nötigen Kanäle eines Runs über die WinTAX-API und wendet
    /// <see cref="StartDetector"/> an (read-only). Zum Vergleich wird der bisherige VCH-Start
    /// (<c>StartTrigger</c>: erstes Mal Gas ≥ Kupplung, Gang ≠ N, kein Begrenzer) mit ausgegeben.
    /// </summary>
    public sealed class StartAnalyzer
    {
        private const double Step = 0.01; // 100 Hz Raster

        private readonly IMarelliDataBrowse _browse;

        public StartAnalyzer(IMarelliDataBrowse browse) => _browse = browse;

        /// <summary>Analysiert alle Runs unter <paramref name="path"/> und gibt eine Tabelle aus.</summary>
        public OracleExitCode Run(string path, StartDetectionOptions? options = null)
        {
            var runs = _browse.IsRunLocation(path)
                ? [path]
                : RunFolders.InOrder(_browse.FindRuns(path) ?? Enumerable.Empty<string>());
            if (runs.Count == 0)
            {
                Console.WriteLine($"Keine gültigen Runs gefunden in: {path}");
                return OracleExitCode.DataNotFound;
            }

            int valid = 0, rejected = 0, legacyOnly = 0;
            foreach (var run in runs)
            {
                var name = $"{Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(run)))}\\{Path.GetFileName(Path.GetDirectoryName(run))}\\{Path.GetFileName(run)}";
                var (signals, missing, legacyStart) = Load(run);
                if (signals is null)
                {
                    Console.WriteLine($"{name,-24} – Kanäle fehlen: {missing}");
                    continue;
                }

                IReadOnlyList<DetectedStart> starts;
                try
                {
                    starts = StartDetector.Detect(signals, options);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{name,-24} – Analyse fehlgeschlagen: {ex.Message}");
                    continue;
                }

                if (double.TryParse(Environment.GetEnvironmentVariable("ORACLE_START_DEBUG"), System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out var debugTime))
                    DumpWindow(name, signals, debugTime);
                var line = $"{name,-24} legacy StartTrigger: {(legacyStart is { } l ? $"{l,7:0.00} s" : "      –")}";
                if (starts.Count == 0)
                {
                    Console.WriteLine($"{line} | kein Start-Kandidat");
                    if (legacyStart is not null) legacyOnly++;
                    continue;
                }

                Console.WriteLine(line);
                foreach (var start in starts)
                {
                    Console.WriteLine($"    {(start.Valid ? "VALID " : "reject")} t={start.TimeSeconds,8:0.00} s  rpm={start.RpmAtLaunch,6:0}  " +
                                      $"vmax3s={start.MaxSpeed3sKph,5:0} km/h  s5s={start.Distance5sMeters,5:0} m  LC={(start.LaunchControl ? "on" : "off")}  {start.Reason}");
                    if (start.Valid) valid++; else rejected++;
                }
            }

            Console.WriteLine($"[detect-start] {runs.Count} Run(s): {valid} gültige Start(s), {rejected} verworfene Kandidaten, {legacyOnly} Run(s) mit altem StartTrigger aber ohne Startkandidat.");
            return OracleExitCode.Success;
        }

        private static (StartSignals? Signals, string Missing, double? LegacyStart) Load(string run)
        {
            var reader = KtmDataHandlerFactory.CreateReader();
            try
            {
                reader.SetDataLocation([run]);
                if (reader.LoadRun() <= 0)
                    return (null, "LoadRun fehlgeschlagen", null);

                var names = reader.GetChannelsNames() ?? [];
                string? Find(params string[] candidates) => ChannelResolver.Find(names, candidates);

                var speed = Find("vVehicleGps", "vVehicleGPS", "vGPS");
                var gear = Find("NGear", "nGear");
                var rpm = Find("nEngine720", "nEngine");
                var throttle = Find("rThrottle1", "rThrottle");
                var clutch = Find("xClutch");
                var rls = Find("revlimitsource", "RevLimitSource");
                var lc = Find("bLaunchActive");
                var missing = string.Join(", ", new[] { ("vVehicleGps", speed), ("NGear", gear), ("nEngine720", rpm), ("rThrottle1", throttle) }
                    .Where(x => x.Item2 is null).Select(x => x.Item1));
                if (missing.Length > 0)
                    return (null, missing, null);

                var request = new[] { speed, gear, rpm, throttle, clutch, rls, lc }.OfType<string>().Distinct().ToArray();
                var ds = reader.GetChannels(request);
                (double[] T, double[] V)? Channel(string? channel)
                {
                    if (channel is null || ds?.GetElement(channel)?.SignalData is not { } signal)
                        return null;
                    return signal.Time is double[] t && signal.Data is double[] v ? (t, v) : null;
                }

                var sp = Channel(speed)!.Value;
                var end = Math.Max(sp.T.Length > 0 ? sp.T[^1] : 0, reader.GetLastSampleTime());
                var grid = StartDetector.Grid(sp.T.Length > 0 ? sp.T[0] : 0, end, Step);
                double[]? On((double[] T, double[] V)? c) => c is { } x ? StartDetector.Resample(x.T, x.V, grid) : null;

                var speedKph = On(sp);
                var gearValues = On(Channel(gear));
                var rpmValues = On(Channel(rpm));
                var throttleValues = On(Channel(throttle));
                if (speedKph is null || gearValues is null || rpmValues is null || throttleValues is null)
                    return (null, "Kanaldaten nicht im erwarteten Format", null);

                var signals = new StartSignals
                {
                    Time = grid,
                    SpeedKph = speedKph,
                    Gear = gearValues,
                    Rpm = rpmValues,
                    Throttle = throttleValues,
                    Clutch = On(Channel(clutch)),
                    RevLimitSource = On(Channel(rls)),
                    LaunchActive = On(Channel(lc)),
                };
                return (signals, string.Empty, LegacyStart(signals));
            }
            catch (Exception ex)
            {
                return (null, ex.Message, null);
            }
            finally
            {
                reader.ReleaseObjects();
            }
        }

        /// <summary>Diagnose (Umgebungsvariable ORACLE_START_DEBUG=&lt;Zeit s&gt;): Signale ±4 s um den Zeitpunkt.</summary>
        private static void DumpWindow(string name, StartSignals s, double around)
        {
            Console.WriteLine($"    --- {name} signals around {around:0.00} s");
            for (var k = 0; k < s.Time.Length; k += 25)
            {
                if (s.Time[k] < around - 4 || s.Time[k] > around + 4) continue;
                Console.WriteLine($"    t={s.Time[k],7:0.00} v={s.SpeedKph[k],5:0.0} gear={s.Gear[k],2:0} rpm={s.Rpm[k],6:0} thr={s.Throttle[k],5:0} " +
                                  $"clutch={(s.Clutch?[k] ?? double.NaN),5:0} rls={(s.RevLimitSource?[k] ?? double.NaN),3:0} lc={(s.LaunchActive?[k] ?? double.NaN),2:0}");
            }
        }

        /// <summary>Bisherige VCH-Definition: erstes Sample mit Gas ≥ Kupplung, Gang ≠ 0, kein Begrenzer (bei Kupplungsdaten).</summary>
        private static double? LegacyStart(StartSignals s)
        {
            if (s.Clutch is null || s.Clutch.All(c => c == 0))
                return null;
            for (var k = 0; k < s.Time.Length; k++)
            {
                if (s.Throttle[k] >= s.Clutch[k] && s.Gear[k] != 0 && (s.RevLimitSource is null || Math.Abs(s.RevLimitSource[k] - 1) > 0.5))
                    return s.Time[k];
            }
            return null;
        }
    }
}
