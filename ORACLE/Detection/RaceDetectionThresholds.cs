namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Thresholds for race detection (<see cref="RaceSegmentDetector"/>).
    /// Loaded from <c>oracle-config.json</c> (section <c>RaceDetection</c>); otherwise defaults.
    ///
    /// <para>Detection is deliberately <b>powertrain-independent</b>: it uses only
    /// GPS (distance from Lat/Lon) and throttle position; RPM and GPS quality serve only as guards.
    /// <c>vRearnEng</c> (computed from RPM × gear ratio) is deliberately NOT used.</para>
    /// </summary>
    public sealed class RaceDetectionThresholds
    {
        /// <summary>Engine counts as "on" from this mean RPM [rpm]. Below: no race.</summary>
        public double EngineOnMinRpm { get; set; } = 2000.0;

        /// <summary>Absolute minimum GPS distance [m] for a run to count as a race.</summary>
        public double DistanceMinMeters { get; set; } = 1000.0;

        /// <summary>
        /// Factor applied to the track length (from KOMPASS). The effective minimum distance is
        /// <c>max(DistanceMinMeters, TrackLengthFactor × TrackLen)</c>, if TrackLen is known.
        /// </summary>
        public double TrackLengthFactor { get; set; } = 0.5;

        /// <summary>Minimum average speed [kph] (GPS distance / duration).</summary>
        public double SpeedMinKph { get; set; } = 25.0;

        /// <summary>Minimum average throttle position [%].</summary>
        public double ThrottleMinPercent { get; set; } = 15.0;

        /// <summary>Fallback on GPS loss: minimum throttle [%] (higher threshold, lower confidence).</summary>
        public double ThrottleFallbackPercent { get; set; } = 20.0;

        /// <summary>Fallback on GPS loss: minimum average RPM [rpm].</summary>
        public double RpmRaceMin { get; set; } = 5000.0;

        /// <summary>Minimum number of valid GPS points for GPS to be considered usable.</summary>
        public int MinValidGpsSamples { get; set; } = 100;
    }
}
