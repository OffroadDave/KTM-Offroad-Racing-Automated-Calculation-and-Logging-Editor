namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Ergebnis der Berechnungspipeline eines Runs: Gesamtdistanz (für die stint-übergreifende
    /// sVehicle-Akkumulation) und die GPS-Distanz je Lap (für den LapDistance-Eintrag).
    /// </summary>
    public sealed class RunCalcResult
    {
        /// <summary>In diesem Run zurückgelegte Distanz [m] (ohne Offset).</summary>
        public double RunDistanceMeters { get; init; }

        /// <summary>GPS-Distanz [m] je Lap (Index 0 = Lap 1), aus Lat_dez/Lon_dez (Haversine).</summary>
        public IReadOnlyList<double> LapDistancesMeters { get; init; } = [];
    }
}
