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
    ///   <item><c>Lat_dez</c>/<c>Lon_dez</c> → GPS-Distanz (Haversine) und daraus abgeleitete Geschwindigkeit</item>
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

        private const double EarthRadiusMeters = 6371000.0;

        /// <summary>
        /// Erstellt einen Detektor mit expliziten Abhängigkeiten (für Unit-Tests).
        /// </summary>
        public RaceSegmentDetector(IMarelliDataBrowse browse, RaceDetectionThresholds thresholds)
        {
            ArgumentNullException.ThrowIfNull(browse);
            ArgumentNullException.ThrowIfNull(thresholds);
            _browse = browse;
            _thresholds = thresholds;
        }

        /// <summary>
        /// Factory: Detektor mit Standard-Abhängigkeiten (Config aus <c>oracle-config.json</c>).
        /// </summary>
        public static RaceSegmentDetector Create()
            => new(new MarelliDataBrowse(), OracleConfig.Load().RaceDetection);

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
                : (_browse.FindRuns(folderPath) ?? Enumerable.Empty<string>())
                    .OrderBy(p => Path.GetFileName(p), StringComparer.OrdinalIgnoreCase)
                    .ToList();

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

            var reader = KtmDataHandlerFactory.CreateReader();
            try
            {
                reader.SetDataLocation([runPath]);
                int loadResult = reader.LoadRun();
                if (loadResult < 0)
                    return ErrorResult(runPath, folder, $"LoadRun fehlgeschlagen (Rückgabe {loadResult})");

                var names = reader.GetChannelsNames() ?? Array.Empty<string>();

                string? latName = Resolve(names, "Lat_dez", "lat");
                string? lonName = Resolve(names, "Lon_dez", "lon");
                string? thrName = Resolve(names, "rThrottle1", "throttle");
                string? rpmName = Resolve(names, "nEngine720", "nengine");
                string? satName = Resolve(names, "ValidSat", "validsat");

                var request = new[] { latName, lonName, thrName, rpmName, satName }
                    .Where(n => n is not null).Cast<string>().Distinct().ToArray();

                double duration = reader.GetLastSampleTime();
                IDataset? ds = request.Length > 0 ? reader.GetChannels(request) : null;

                var lat = GetData(ds, latName);
                var lon = GetData(ds, lonName);
                var thr = GetData(ds, thrName);
                var rpm = GetData(ds, rpmName);
                var sat = GetData(ds, satName);

                double distance = GpsDistance(lat, lon, out int validGps);
                double meanSpeedKph = duration > 1.0 ? distance / duration * 3.6 : 0.0;
                double meanThrottle = Mean(thr);
                double meanRpm = Mean(rpm);
                double meanSat = Mean(sat);
                bool gpsUsable = meanSat > 0.0 && validGps > _thresholds.MinValidGpsSamples;

                double minDistance = trackLengthMeters > 0
                    ? Math.Max(_thresholds.DistanceMinMeters, _thresholds.TrackLengthFactor * trackLengthMeters)
                    : _thresholds.DistanceMinMeters;

                var (verdict, reason, lowConfidence) =
                    Classify(distance, meanSpeedKph, meanThrottle, meanRpm, gpsUsable, minDistance);

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
        /// </summary>
        private (RaceRunVerdict Verdict, string Reason, bool LowConfidence) Classify(
            double distanceM, double speedKph, double throttlePct, double rpm, bool gpsUsable, double minDistanceM)
        {
            // Guard: Motor aus → niemals Rennen (fängt Blips/Stillstand ab).
            if (rpm < _thresholds.EngineOnMinRpm)
                return (RaceRunVerdict.NoRace, "Motor aus", false);

            // Primärpfad: GPS gültig → Distanz + Geschwindigkeit + Gas (UND-verknüpft).
            if (gpsUsable)
            {
                if (distanceM <= minDistanceM)
                    return (RaceRunVerdict.NoRace, $"Distanz {distanceM:F0} m <= {minDistanceM:F0} m", false);
                if (speedKph <= _thresholds.SpeedMinKph)
                    return (RaceRunVerdict.NoRace, $"vGPS {speedKph:F0} kph zu langsam", false);
                if (throttlePct <= _thresholds.ThrottleMinPercent)
                    return (RaceRunVerdict.NoRace, $"Gas {throttlePct:F0} % zu gering", false);
                return (RaceRunVerdict.Race, "GPS: Distanz+Speed+Gas erfüllt", false);
            }

            // Fallback: GPS ausgefallen → nur Gas + Drehzahl (geringere Konfidenz).
            if (throttlePct > _thresholds.ThrottleFallbackPercent && rpm > _thresholds.RpmRaceMin)
                return (RaceRunVerdict.Race, "Fallback Gas+Drehzahl (GPS ungültig)", true);

            return (RaceRunVerdict.NoRace, "GPS ungültig, kein Renn-Profil", false);
        }

        /// <summary>Kanalname per exaktem Namen, sonst per Keyword (case-insensitive) auflösen.</summary>
        private static string? Resolve(string[] names, string exact, string keyword)
            => names.FirstOrDefault(n => n.Equals(exact, StringComparison.OrdinalIgnoreCase))
            ?? names.FirstOrDefault(n => n.ToLowerInvariant().Contains(keyword));

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
        /// Kumulierte Distanz über gültige aufeinanderfolgende GPS-Punkte (Haversine).
        /// Nullpunkte (Dropouts) werden übersprungen, nicht interpoliert.
        /// </summary>
        private static double GpsDistance(double[]? lat, double[]? lon, out int validSamples)
        {
            validSamples = 0;
            if (lat is null || lon is null) return 0.0;

            int n = Math.Min(lat.Length, lon.Length);
            double distance = 0.0;
            for (int i = 1; i < n; i++)
            {
                if (lat[i] == 0 || lon[i] == 0 || lat[i - 1] == 0 || lon[i - 1] == 0) continue;

                double la1 = lat[i - 1] * Math.PI / 180.0, la2 = lat[i] * Math.PI / 180.0;
                double dLat = la2 - la1;
                double dLon = (lon[i] - lon[i - 1]) * Math.PI / 180.0;
                double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                         + Math.Cos(la1) * Math.Cos(la2) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                distance += EarthRadiusMeters * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                validSamples++;
            }
            return distance;
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
