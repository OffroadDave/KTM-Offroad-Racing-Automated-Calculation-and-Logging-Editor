namespace Ktm.Racing.Core
{
    /// <summary>Verdikt der Renn-Erkennung für einen einzelnen Run.</summary>
    public enum RaceRunVerdict
    {
        /// <summary>Run wurde als Rennen (Renntempo) erkannt.</summary>
        Race,

        /// <summary>Run ist kein Rennen (Stillstand, Rollen, Auslauf, Sichtungsrunde).</summary>
        NoRace,

        /// <summary>Run konnte nicht klassifiziert werden (Ladefehler, kein Zugriff).</summary>
        Error,
    }

    /// <summary>
    /// Berechnete Kennzahlen und Klassifikations-Ergebnis eines einzelnen Runs.
    /// Read-only Momentaufnahme — verändert keine Daten.
    /// </summary>
    public sealed class RunRaceFeatures
    {
        /// <summary>Vollständiger Pfad zum Run-Ordner.</summary>
        public string RunPath { get; init; } = "";

        /// <summary>Run-Ordnername (z.B. "Run_1083").</summary>
        public string RunFolderName { get; init; } = "";

        /// <summary>Laufzeit des Runs [s] (<c>GetLastSampleTime</c>).</summary>
        public double DurationSeconds { get; init; }

        /// <summary>Zurückgelegte GPS-Distanz [m] (Haversine über Lat_dez/Lon_dez).</summary>
        public double GpsDistanceMeters { get; init; }

        /// <summary>Durchschnittsgeschwindigkeit [kph] = GPS-Distanz / Laufzeit.</summary>
        public double MeanGpsSpeedKph { get; init; }

        /// <summary>Durchschnittliche Gasstellung [%] (<c>rThrottle1</c>).</summary>
        public double MeanThrottlePercent { get; init; }

        /// <summary>Durchschnittliche Motordrehzahl [rpm] (<c>nEngine720</c>) — nur Guard.</summary>
        public double MeanEngineRpm { get; init; }

        /// <summary>Mittelwert <c>ValidSat</c> — GPS-Verfügbarkeits-Guard.</summary>
        public double MeanValidSat { get; init; }

        /// <summary>Anzahl gültiger GPS-Punkte (Lat/Lon ≠ 0).</summary>
        public int ValidGpsSamples { get; init; }

        /// <summary>True wenn GPS als vertrauenswürdig gilt (sonst greift der Fallback).</summary>
        public bool GpsUsable { get; init; }

        /// <summary>Klassifikations-Verdikt.</summary>
        public RaceRunVerdict Verdict { get; init; }

        /// <summary>Menschlich lesbare Begründung des Verdikts.</summary>
        public string Reason { get; init; } = "";

        /// <summary>True wenn das Verdikt über den GPS-Fallback zustande kam (geringere Konfidenz).</summary>
        public bool LowConfidence { get; init; }
    }

    /// <summary>
    /// Ergebnis der Renn-Erkennung über einen Rider-/Session-Ordner (oder einen einzelnen Run).
    /// </summary>
    public sealed class RaceDetectionResult
    {
        /// <summary>Analysierter Eingabepfad.</summary>
        public string RiderPath { get; init; } = "";

        /// <summary>Kennzahlen/Verdikt pro entdecktem Run (in Run-Reihenfolge).</summary>
        public IReadOnlyList<RunRaceFeatures> Runs { get; init; } = [];

        /// <summary>Pfade der als Rennen erkannten Runs.</summary>
        public IReadOnlyList<string> RaceRunPaths { get; init; } = [];

        /// <summary>Anzahl als Rennen erkannter Runs.</summary>
        public int RaceCount => RaceRunPaths.Count;
    }
}
