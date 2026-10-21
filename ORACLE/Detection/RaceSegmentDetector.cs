using Ktm.Racing.Domain.Telemetry;
using KtmDataHandler.Marelli.Wintax;
using KtmDataHandler.Models.Channels;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Detects which runs of a rider/session folder are actually a <b>race</b> (race pace)
    /// – as opposed to engine blips, idling, slow rolling, sighting laps or cool-down laps.
    ///
    /// <para><b>Background:</b> The logger increments its counter on every engine start, so a
    /// race interrupted by a crash/stall is fragmented across several <c>Run_xxx</c> folders.
    /// Time gaps alone do not reliably separate races from cool-down laps (these follow
    /// seconds later). Motion/intensity signals, on the other hand, separate them sharply.</para>
    ///
    /// <para><b>Signals (fully powertrain-independent):</b></para>
    /// <list type="bullet">
    ///   <item><c>Lat_dez</c>/<c>Lon_dez</c> → GPS distance (cleaned by GpsCleaner) and the speed derived from it</item>
    ///   <item><c>rThrottle1</c> → mean throttle position (riding intensity, rules out touring/sighting)</item>
    ///   <item><c>ValidSat</c> → GPS quality guard</item>
    ///   <item><c>nEngine720</c> → engine-on guard (catches dead blips)</item>
    /// </list>
    /// <c>vRearnEng</c> is deliberately NOT used (computed from RPM × gear ratio,
    /// unreliable with the clutch pulled, a spinning or a locked wheel).
    /// </summary>
    public class RaceSegmentDetector
    {
        private readonly IMarelliDataBrowse _browse;
        private readonly RaceDetectionThresholds _thresholds;
        private readonly GpsSettings _gps;

        /// <summary>
        /// Creates a detector with explicit dependencies (for unit tests).
        /// </summary>
        public RaceSegmentDetector(IMarelliDataBrowse browse, RaceDetectionThresholds thresholds, GpsSettings? gps = null)
        {
            ArgumentNullException.ThrowIfNull(browse);
            ArgumentNullException.ThrowIfNull(thresholds);
            _browse = browse;
            _thresholds = thresholds;
            _gps = gps ?? new GpsSettings();
        }

        /// <summary>GPS channels and cleaning settings that also apply to the calculations of the race runs.</summary>
        public GpsSettings Gps => _gps;

        /// <summary>
        /// Classifies all runs below a rider/session folder (or a single run).
        /// </summary>
        /// <param name="folderPath">Rider/session folder or a direct run folder.</param>
        /// <param name="trackLengthMeters">Optional track length from KOMPASS for the relative distance threshold (0 = absolute only).</param>
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
        /// Loads the required channels of a single run, computes the metrics and classifies it.
        /// Errors result in <see cref="RaceRunVerdict.Error"/> instead of an exception (fail-forward).
        /// </summary>
        public RunRaceFeatures ClassifyRun(string runPath, double trackLengthMeters = 0)
        {
            string folder = Path.GetFileName(
                runPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

            if (!Directory.Exists(runPath))
                return ErrorResult(runPath, folder, "Run folder does not exist");

            var reader = KtmDataHandlerFactory.CreateAccess();
            try
            {
                reader.SetDataLocation([runPath]);
                int loadResult = reader.LoadRun();
                if (loadResult <= 0)
                    return ErrorResult(runPath, folder, $"LoadRun failed (return value {loadResult})");

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

                // Missing channels are "unknown" (NaN), not 0: a different channel setup must not discard a race.
                var missing = new[] { ("nEngine720", rpm), ("rThrottle1", thr), ("ValidSat", sat) }
                    .Where(c => c.Item2 is null).Select(c => c.Item1).ToList();
                if (missing.Count > 0)
                    Console.WriteLine($"[detect] {folder}: channel missing ({string.Join(", ", missing)}) – criterion skipped.");

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
        //  Classification & helper methods
        // =====================================================================

        /// <summary>
        /// Three-stage classification: engine guard → GPS path → fallback on GPS loss.
        /// Missing channels are passed as <see cref="double.NaN"/>; an unknown criterion does not reject.
        /// </summary>
        public static (RaceRunVerdict Verdict, string Reason, bool LowConfidence) Classify(RaceDetectionThresholds thresholds,
            double distanceM, double speedKph, double throttlePct, double rpm, bool gpsUsable, double minDistanceM)
        {
            ArgumentNullException.ThrowIfNull(thresholds);

            bool rpmKnown = !double.IsNaN(rpm);
            bool throttleKnown = !double.IsNaN(throttlePct);

            // Guard: engine off → never a race (catches blips/standstill).
            if (rpm < thresholds.EngineOnMinRpm)
                return (RaceRunVerdict.NoRace, "Engine off", false);

            // Primary path: GPS valid → distance + speed + throttle (AND-combined).
            if (gpsUsable)
            {
                if (distanceM <= minDistanceM)
                    return (RaceRunVerdict.NoRace, $"Distance {distanceM:F0} m <= {minDistanceM:F0} m", false);
                if (speedKph <= thresholds.SpeedMinKph)
                    return (RaceRunVerdict.NoRace, $"vGPS {speedKph:F0} kph too slow", false);
                if (throttlePct <= thresholds.ThrottleMinPercent)
                    return (RaceRunVerdict.NoRace, $"Throttle {throttlePct:F0} % too low", false);
                return (RaceRunVerdict.Race, "GPS: distance+speed+throttle met", !rpmKnown || !throttleKnown);
            }

            // Fallback: GPS lost → throttle + RPM only (lower confidence).
            // Minimum evidence: without GPS at least throttle or RPM must be present; a missing channel is skipped.
            if (!rpmKnown && !throttleKnown)
                return (RaceRunVerdict.NoRace, "GPS invalid, throttle and RPM missing – no race evidence", false);

            bool throttleOk = !throttleKnown || throttlePct > thresholds.ThrottleFallbackPercent;
            bool rpmOk = !rpmKnown || rpm > thresholds.RpmRaceMin;
            if (throttleOk && rpmOk)
            {
                string reason = rpmKnown && throttleKnown
                    ? "Fallback throttle+RPM (GPS invalid)"
                    : rpmKnown
                        ? "Fallback RPM only (GPS invalid, throttle missing)"
                        : "Fallback throttle only (GPS invalid, RPM missing)";
                return (RaceRunVerdict.Race, reason, true);
            }

            return (RaceRunVerdict.NoRace, "GPS invalid, no race profile", false);
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
        /// GPS distance over the cleaned fixes (<see cref="GpsCleaner"/>: dropouts bridged like Lat_Valid,
        /// jumps not counted). <paramref name="validSamples"/> = samples with a valid fix.
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
