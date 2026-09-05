namespace Ktm.Racing.Core;

/// <summary>Ein berechneter Ausgabewert: Spaltenname (Alias), Einheit, Wert (null = nicht verfügbar)
/// und optional die Nachkommastellen laut VCH-Einstellung (für die Zellformatierung).</summary>
public sealed record RaceReportValue(string Alias, string Unit, double? Value, int? Decimals = null);

/// <summary>
/// Ergebnis eines Race Reports: Metadaten (Track/Session/Driver/Device/Datum) und die berechneten Werte.
/// </summary>
public sealed class RaceReportResult
{
    public string Track { get; init; } = "";
    public string Session { get; init; } = "";
    public string Driver { get; init; } = "";
    public string Device { get; init; } = "";
    public DateTime? SessionDate { get; init; }

    /// <summary>Berechnete Kanäle in Ausgabereihenfolge.</summary>
    public IReadOnlyList<RaceReportValue> Values { get; init; } = Array.Empty<RaceReportValue>();

    /// <summary>Namen der verarbeiteten Runs (für Anzeige/Log).</summary>
    public IReadOnlyList<string> RunNames { get; init; } = Array.Empty<string>();

    /// <summary>Warnungen (z.B. leerer Driver, fehlende Kanäle).</summary>
    public IReadOnlyList<string> Warnings { get; init; } = Array.Empty<string>();
}
