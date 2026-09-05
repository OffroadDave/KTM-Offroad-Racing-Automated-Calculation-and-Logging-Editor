namespace Ktm.Racing.Core;

/// <summary>
/// Ergebnis der Run-Discovery: ein einzelner Run mit den für die Auswahl relevanten Metadaten.
/// Wird von <see cref="RunDiscoveryService"/> erzeugt und in der GUI angezeigt.
/// </summary>
public sealed class DiscoveredRun
{
    /// <summary>Vollständiger Pfad zum Run-Ordner (z.B. ...\R1\AA 01\Run_1078).</summary>
    public required string RunPath { get; init; }

    /// <summary>Ordnername des Runs (z.B. "Run_1078").</summary>
    public required string RunName { get; init; }

    /// <summary>Track-Name aus Session.xml (z.B. "RT22_GER-Teutschenthal").</summary>
    public string Track { get; init; } = "";

    /// <summary>Session-Name aus Session.xml (z.B. "R1").</summary>
    public string Session { get; init; } = "";

    /// <summary>Device/Bike aus Car.xml (z.B. "AA 01").</summary>
    public string Device { get; init; } = "";

    /// <summary>Fahrername aus Car.xml (im Rohzustand oft leer, bis ORACLE lief).</summary>
    public string Driver { get; init; } = "";

    /// <summary>Anzahl Laps im Run (aus RunLapHeader.xml bzw. Lap_*-Ordnern).</summary>
    public int LapCount { get; init; }

    /// <summary>Gesamtdauer des Runs = Summe aller Lap-Zeiten (aus RunLapHeader.xml, LapTime-Feld).</summary>
    public TimeSpan? Duration { get; init; }

    /// <summary>Echter Aufnahmezeitpunkt = frühestes AcquireTime aus RunLapHeader.xml; kopiersicher (anders als ModT).</summary>
    public DateTime? AcquisitionTime { get; init; }

    /// <summary>Akquisitions-Reihenfolge = kleinster Lap-Ordner-Zähler (Lap_NNNNNNNN_...);
    /// stabiler, kopiersicherer Sortier- und Anzeigeschlüssel (ersetzt die unzuverlässige Startzeit).</summary>
    public long? AcquisitionOrder { get; init; }
}
