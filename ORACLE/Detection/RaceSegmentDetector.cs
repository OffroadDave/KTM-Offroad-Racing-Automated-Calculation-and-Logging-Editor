using Ktm.Racing.Domain.Telemetry;
using KtmDataHandler.Marelli.Wintax;
using KtmDataHandler.Models.Channels;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Erkennt, welche Runs eines Rider-/Session-Ordners tatsächlich ein <b>Rennen</b> (Renntempo)
    /// sind – im Gegensatz zu Motor-Blips, Standgas, langsamem Rollen, Sichtungsrunden oder Auslauf.
    ///
    /// <para><b>Hintergrund:</b> Bei jedem Motorstart erhöht der Logger den Zähler, sodass ein
    /// durch Sturz/Abwürgen unterbrochenes Rennen über mehrere <c>Run_xxx</c>-Ordner fragmentiert.
    /// Zeitlücken allein trennen Rennen nicht zuverlässig von Auslaufrunden (diese folgen mit
    /// Sekundenabstand). Bewegungs-/Intensitäts-Signale trennen dagegen scharf.</para>
    ///
    /// <para><b>Signale (rein antriebsstrang-unabhängig):</b></para>
    /// <list type="bullet">
    ///   <item><c>Lat_dez</c>/<c>Lon_dez</c> → GPS-Distanz (aufbereitet durch GpsCleaner) und daraus abgeleitete Geschwindigkeit</item>
    ///   <item><c>rThrottle1</c> → mittlere Gasstellung (Fahr-Intensität, schließt Touring/Sichtung aus)</item>
    ///   <item><c>ValidSat</c> → GPS-Qualitäts-Guard</item>
    ///   <item><c>nEngine720</c> → Motor-an-Guard (fängt tote Blips ab)</item>
    /// </list>
    /// <c>vRearnEng</c> wird bewusst NICHT genutzt (aus Drehzahl × Getriebeübersetzung berechnet,
    /// unzuverlässig bei gezogener Kupplung, durchdrehendem oder blockiertem Rad).
    /// </summary>
    public class RaceSegmentDetector
    {
        private readonly IMarelliDataBrowse _browse;
        private readonly RaceDetectionThresholds _thresholds;
        private readonly GpsSettings _gps;

        /// <summary>
        /// Erstellt einen Detektor mit expliziten Abhängigkeiten (für Unit-Tests).
        /// </summary>
        public RaceSegmentDetector(IMarelliDataBrowse browse, RaceDetectionThresholds thresholds, GpsSettings? gps = null)
        {
            ArgumentNullException.ThrowIfNull(browse);
            ArgumentNullException.ThrowIfNull(thresholds);
            _browse = browse;
            _thresholds = thresholds;
            _gps = gps ?? new GpsSettings();
        }

        /// <summary>GPS-Kanäle und Aufbereitung, die auch für die Berechnungen der Renn-Runs gelten.</summary>
        public GpsSettings Gps => _gps;

        /// <summary>
        /// Klassifiziert alle Runs unterhalb eines Rider-/Session-Ordners (oder einen einzelnen Run).
        /// </summary>
        /// <param name="folderPath">Rider-/Session-Ordner oder direkter Run-Ordner.</param>
        /// <param name="trackLengthMeters">Optionale Track-Länge aus KOMPASS für die relative Distanzschwelle (0 = nur absolut).</param>
        public RaceDetectionResult DetectRaces(string folderPath, double trackLengthMeters = 0)
        {
            if (!Directory.Exists(folderPath))
            {
                return new RaceDetectionResult { RiderPath = folderPath };
            }

            List<string> runPaths = _browse.IsRunLocation(folderPath)
                ? [folderPath]
                : RunFolders.InOrder(_browse.FindRuns(folderPath) ?? Enumerable.Empty<string>());

            var features = new List<RunRaceFeatures>(runPaths.Count);
            foreach (var runPath in runPaths)
                features.Add(ClassifyRun(runPath, trackLengthMeters));

            var raceRunPaths = features
                .Where(f => f.Verdict == RaceRunVerdict.Race)
                .Select(f => f.RunPath)
                .ToList();

            return new RaceDetectionResult
            {
                RiderPath = folderPath,
                Runs = features.AsReadOnly(),
                RaceRunPaths = raceRunPaths.AsReadOnly(),
            };
        }

        /// <summary>
        /// Lädt die notwendigen Kanäle eines einzelnen Runs, berechnet die Kennzahlen und klassifiziert ihn.
        /// Fehler führen zu <see cref="RaceRunVerdict.Error"/> statt zu einer Exception (Fail-Forward).
        /// </summary>
        public RunRaceFeatures ClassifyRun(string runPath, double trackLengthMeters = 0)
        {
            string folder = Path.GetFileName(
                runPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

            if (!Directory.Exists(runPath))
                return ErrorResult(runPath, folder, "Run-Ordner existiert nicht");

            var reader = KtmDataHandlerFactory.CreateAccess();
            try
            {
                reader.SetDataLocation([runPath]);
                int loadResult = reader.LoadRun();
                if (loadResult <= 0)
                    return ErrorResult(runPath, folder, $"LoadRun fehlgeschlagen (Rückgabe {loadResult})");

                var names = reader.GetChannelsNames() ?? Array.Empty<string>();

                string? latName = GpsSettings.Resolve(names, _gps.LatitudeChannel);
                string? lonName = GpsSettings.Resolve(names, _gps.LongitudeChannel);
                string? thrName = ChannelResolver.Find(names, "rThrottle1", "rThrottle");
                string? rpmName = ChannelResolver.Find(names, "nEngine720", "nEngine");
                string? satName = ChannelResolver.Find(names, "ValidSat");

                var request = new[] { latName, lonName, thrName, rpmName, satName }
                    .Where(n => n is not null).Cast<string>().Distinct().ToArray();

                double duration = reader.GetLastSampleTime();
                IDataset? ds = request.Length > 0 ? reader.GetChannels(request) : null;

                var latSignal = latName is null ? null : ds?.GetElement(latName)?.SignalData;
                var lonSignal = lonName is null ? null : ds?.GetElement(lonName)?.SignalData;
                var data = new RunGpsData
                {
                    ChannelNames = names,
                    LapMarks = reader.GetLapMarks() ?? [],
                    Time = latSignal?.Time as double[],
                    Latitude = latSignal?.Data as double[],
                    Longitude = lonSignal?.Data as double[],
                };

                var thr = GetData(ds, thrName);
                var rpm = GetData(ds, rpmName);
                var sat = GetData(ds, satName);

                // Fehlende Kanäle sind "unbekannt" (NaN), nicht 0: ein anderes Kanal-Setup darf ein Rennen nicht verwerfen.
                var missing = new[] { ("nEngine720", rpm), ("rThrottle1", thr), ("ValidSat", sat) }
                    .Where(c => c.Item2 is null).Select(c => c.Item1).ToList();
                if (missing.Count > 0)
                    Console.WriteLine($"[detect] {folder}: Kanal fehlt ({string.Join(", ", missing)}) – Kriterium wird übersprungen.");

                double distance = GpsDistance(ds, latName, lonName, out int validGps);
                double meanSpeedKph = duration > 1.0 ? distance / duration * 3.6 : 0.0;
                double meanThrottle = thr is null ? double.NaN : Mean(thr);
                double meanRpm = rpm is null ? double.NaN : Mean(rpm);
                double meanSat = sat is null ? double.NaN : Mean(sat);
                bool gpsUsable = (sat is null || meanSat > 0.0) && validGps > _thresholds.MinValidGpsSamples;

                double minDistance = trackLengthMeters > 0
                    ? Math.Max(_thresholds.DistanceMinMeters, _thresholds.TrackLengthFactor * trackLengthMeters)
                    : _thresholds.DistanceMinMeters;

                var (verdict, reason, lowConfidence) =
                    Classify(_thresholds, distance, meanSpeedKph, meanThrottle, meanRpm, gpsUsable, minDistance);
                lowConfidence |= missing.Count > 0 && verdict == RaceRunVerdict.Race;

                return new RunRaceFeatures
                {
                    RunPath = runPath,
                    RunFolderName = folder,
                    DurationSeconds = duration,
                    GpsDistanceMeters = distance,
                    MeanGpsSpeedKph = meanSpeedKph,
                    MeanThrottlePercent = meanThrottle,
                    MeanEngineRpm = meanRpm,
                    MeanValidSat = meanSat,
                    ValidGpsSamples = validGps,
                    GpsUsable = gpsUsable,
                    Verdict = verdict,
                    Reason = reason,
                    LowConfidence = lowConfidence,
                    Data = data,
                };
            }
            catch (Exception ex)
            {
                return ErrorResult(runPath, folder, ex.Message);
            }
            finally
            {
                reader.ReleaseObjects();
            }
        }

        // =====================================================================
        //  Klassifikation & Hilfsmethoden
        // =====================================================================

        /// <summary>
        /// Dreistufige Klassifikation: Motor-Guard → GPS-Pfad → Fallback bei GPS-Ausfall.
        /// Fehlende Kanäle werden als <see cref="double.NaN"/> übergeben; ein unbekanntes Kriterium verwirft nicht.
        /// </summary>
        public static (RaceRunVerdict Verdict, string Reason, bool LowConfidence) Classify(RaceDetectionThresholds thresholds,
            double distanceM, double speedKph, double throttlePct, double rpm, bool gpsUsable, double minDistanceM)
        {
            ArgumentNullException.ThrowIfNull(thresholds);

            bool rpmKnown = !double.IsNaN(rpm);
            bool throttleKnown = !double.IsNaN(throttlePct);

            // Guard: Motor aus → niemals Rennen (fängt Blips/Stillstand ab).
            if (rpm < thresholds.EngineOnMinRpm)
                return (RaceRunVerdict.NoRace, "Motor aus", false);

            // Primärpfad: GPS gültig → Distanz + Geschwindigkeit + Gas (UND-verknüpft).
            if (gpsUsable)
            {
                if (distanceM <= minDistanceM)
                    return (RaceRunVerdict.NoRace, $"Distanz {distanceM:F0} m <= {minDistanceM:F0} m", false);
                if (speedKph <= thresholds.SpeedMinKph)
                    return (RaceRunVerdict.NoRace, $"vGPS {speedKph:F0} kph zu langsam", false);
                if (throttlePct <= thresholds.ThrottleMinPercent)
                    return (RaceRunVerdict.NoRace, $"Gas {throttlePct:F0} % zu gering", false);
                return (RaceRunVerdict.Race, "GPS: Distanz+Speed+Gas erfüllt", !rpmKnown || !throttleKnown);
            }

            // Fallback: GPS ausgefallen → nur Gas + Drehzahl (geringere Konfidenz).
            // Mindest-Evidenz: ohne GPS muss wenigstens Gas oder Drehzahl vorhanden sein; ein fehlender Kanal wird übersprungen.
            if (!rpmKnown && !throttleKnown)
                return (RaceRunVerdict.NoRace, "GPS ungültig, Gas und Drehzahl fehlen – keine Renn-Evidenz", false);

            bool throttleOk = !throttleKnown || throttlePct > thresholds.ThrottleFallbackPercent;
            bool rpmOk = !rpmKnown || rpm > thresholds.RpmRaceMin;
            if (throttleOk && rpmOk)
            {
                string reason = rpmKnown && throttleKnown
                    ? "Fallback Gas+Drehzahl (GPS ungültig)"
                    : rpmKnown
                        ? "Fallback nur Drehzahl (GPS ungültig, Gas fehlt)"
                        : "Fallback nur Gas (GPS ungültig, Drehzahl fehlt)";
                return (RaceRunVerdict.Race, reason, true);
            }

            return (RaceRunVerdict.NoRace, "GPS ungültig, kein Renn-Profil", false);
        }

        private static double[]? GetData(IDataset? ds, string? name)
        {
            if (ds is null || name is null) return null;
            return ds.GetElement(name)?.SignalData?.Data as double[];
        }

        private static double Mean(double[]? data)
        {
            if (data is null || data.Length == 0) return 0.0;
            double sum = 0; int count = 0;
            foreach (var v in data)
                if (!double.IsNaN(v) && !double.IsInfinity(v)) { sum += v; count++; }
            return count == 0 ? 0.0 : sum / count;
        }

        /// <summary>
        /// GPS-Distanz über die aufbereiteten Fixes (<see cref="GpsCleaner"/>: Aussetzer überbrückt wie Lat_Valid,
        /// Sprünge nicht mitgezählt). <paramref name="validSamples"/> = Samples mit gültigem Fix.
        /// </summary>
        private double GpsDistance(IDataset? ds, string? latName, string? lonName, out int validSamples)
        {
            validSamples = 0;
            if (ds is null || latName is null || lonName is null) return 0.0;
            var latSignal = ds.GetElement(latName)?.SignalData;
            var lonSignal = ds.GetElement(lonName)?.SignalData;
            if (latSignal?.Data is not double[] lat || lonSignal?.Data is not double[] lon || latSignal.Time is not double[] time)
                return 0.0;

            var cleaned = GpsCleaner.Clean(new GpsTrace(time, lat, lon), _gps.ToCleanOptions());
            validSamples = cleaned.ValidSamples;
            return cleaned.TotalMeters;
        }
        private static RunRaceFeatures ErrorResult(string runPath, string folder, string reason)
            => new()
            {
                RunPath = runPath,
                RunFolderName = folder,
                Verdict = RaceRunVerdict.Error,
                Reason = reason,
            };
    }
}
