namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Result of a run's calculation pipeline: total distance (for the cross-stint
    /// sVehicle accumulation) and the GPS distance per lap (for the LapDistance entry).
    /// </summary>
    public sealed class RunCalcResult
    {
        /// <summary>Distance [m] covered in this run (without offset).</summary>
        public double RunDistanceMeters { get; init; }

        /// <summary>GPS distance [m] per lap (index 0 = lap 1), from sVehicle at the lap boundaries; NaN = unknown (no GPS fix).</summary>
        public IReadOnlyList<double> LapDistancesMeters { get; init; } = [];
    }
}
