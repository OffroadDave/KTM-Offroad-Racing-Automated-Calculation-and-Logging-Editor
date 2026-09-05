namespace Ktm.Racing.Core;

/// <summary>
/// Anzeige-/Formateinstellungen eines Virtual Channels laut VCH-Bibliotheksdatei:
/// Einheit (Measure), Nachkommastellen (Decimals; null = "Auto"), Ausgabeformat (OutFormat) und Frequenz (Freq).
/// </summary>
public sealed record VchChannelSettings(string Unit, int? Decimals, string OutFormat, string Frequency);
