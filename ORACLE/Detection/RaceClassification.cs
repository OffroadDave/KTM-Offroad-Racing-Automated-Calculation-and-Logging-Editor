namespace Ktm.Racing.Oracle
{
    /// <summary>Race detection verdict for a single run.</summary>
    public enum RaceRunVerdict
    {
        /// <summary>Run was detected as a race (race pace).</summary>
        Race,

        /// <summary>Run is not a race (standstill, rolling, cool-down, sighting lap).</summary>
        NoRace,

        /// <summary>Run could not be classified (load error, no access).</summary>
        Error,
    }

    /// <summary>
    /// Computed metrics and classification result of a single run.
    /// Read-only snapshot — does not modify any data.
    /// </summary>
    public sealed class RunRaceFeatures
    {
        /// <summary>Full path to the run folder.</summary>
        public string RunPath { get; init; } = "";

        /// <summary>Run folder name (e.g. "Run_1083").</summary>
        public string RunFolderName { get; init; } = "";

        /// <summary>Duration of the run [s] (<c>GetLastSampleTime</c>).</summary>
        public double DurationSeconds { get; init; }

        /// <summary>GPS distance covered [m] (from Lat_dez/Lon_dez, cleaned by GpsCleaner).</summary>
        public double GpsDistanceMeters { get; init; }

        /// <summary>Average speed [kph] = GPS distance / duration.</summary>
        public double MeanGpsSpeedKph { get; init; }

        /// <summary>Average throttle position [%] (<c>rThrottle1</c>).</summary>
        public double MeanThrottlePercent { get; init; }

        /// <summary>Average engine RPM [rpm] (<c>nEngine720</c>) — guard only.</summary>
        public double MeanEngineRpm { get; init; }

        /// <summary>Mean of <c>ValidSat</c> — GPS availability guard.</summary>
        public double MeanValidSat { get; init; }

        /// <summary>Number of valid GPS points (Lat/Lon ≠ 0).</summary>
        public int ValidGpsSamples { get; init; }

        /// <summary>True if GPS is considered trustworthy (otherwise the fallback applies).</summary>
        public bool GpsUsable { get; init; }

        /// <summary>Classification verdict.</summary>
        public RaceRunVerdict Verdict { get; init; }

        /// <summary>Human-readable reason for the verdict.</summary>
        public string Reason { get; init; } = "";

        /// <summary>True if the verdict was reached via the GPS fallback (lower confidence).</summary>
        public bool LowConfidence { get; init; }

        /// <summary>
        /// Already loaded run data (channel list, lap boundaries, GPS) for the subsequent calculation – avoids loading
        /// the run a second time. <c>null</c> on errors.
        /// </summary>
        public RunGpsData? Data { get; init; }
    }

    /// <summary>
    /// Result of race detection over a rider/session folder (or a single run).
    /// </summary>
    public sealed class RaceDetectionResult
    {
        /// <summary>Analyzed input path.</summary>
        public string RiderPath { get; init; } = "";

        /// <summary>Metrics/verdict per discovered run (in run order).</summary>
        public IReadOnlyList<RunRaceFeatures> Runs { get; init; } = [];

        /// <summary>Paths of the runs detected as races.</summary>
        public IReadOnlyList<string> RaceRunPaths { get; init; } = [];

        /// <summary>Number of runs detected as races.</summary>
        public int RaceCount => RaceRunPaths.Count;
    }
}
