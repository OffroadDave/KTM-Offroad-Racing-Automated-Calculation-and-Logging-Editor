namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// The parts of a run that the calculations need (channel list, lap marks and the raw GPS channels), read once by the
    /// race detection and handed to <see cref="ImportDataCalculator"/> so the run is not loaded a second time.
    /// </summary>
    public sealed class RunGpsData
    {
        /// <summary>All channel names of the run.</summary>
        public IReadOnlyList<string> ChannelNames { get; init; } = [];

        /// <summary>Lap marks of the run (s); empty when unknown.</summary>
        public IReadOnlyList<double> LapMarks { get; init; } = [];

        /// <summary>Sample times of the latitude channel (s); null when the GPS channels are missing.</summary>
        public double[]? Time { get; init; }

        /// <summary>Raw latitude samples (decimal degrees, 0 = no fix).</summary>
        public double[]? Latitude { get; init; }

        /// <summary>Raw longitude samples (decimal degrees, 0 = no fix).</summary>
        public double[]? Longitude { get; init; }
    }
}
