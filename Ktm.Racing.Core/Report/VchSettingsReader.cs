using System.Text;
using System.Xml.Linq;

namespace Ktm.Racing.Core;

/// <summary>
/// Liest die pro Kanal in den VCH-Bibliotheken hinterlegten Einstellungen
/// (Measure/Einheit, Decimals, OutFormat, Freq). Diese steuern die Formatierung der Report-Werte.
///
/// Die eigentliche <b>Berechnung</b> der Kanäle erfolgt weiterhin über die dokumentierte
/// KtmDataHandler-Mathematik (SetMathLibrary + LoadRun + GetChannels); hier werden ausschließlich
/// die in den VCH-Dateien getroffenen Anzeige-Einstellungen ausgelesen.
/// </summary>
public sealed class VchSettingsReader
{
    /// <summary>Baut eine Map Kanalname → <see cref="VchChannelSettings"/> (erste Definition gewinnt).</summary>
    public IReadOnlyDictionary<string, VchChannelSettings> Read(IEnumerable<string> vchFiles)
    {
        var map = new Dictionary<string, VchChannelSettings>(StringComparer.Ordinal);
        foreach (var file in vchFiles)
        {
            if (!File.Exists(file)) continue;

            XDocument document;
            try { document = XDocument.Parse(ReadText(file)); }
            catch { continue; } // Beschädigte/unlesbare VCH-Datei überspringen.

            foreach (var vch in document.Descendants("VCH"))
            {
                var name = vch.Element("Name")?.Value?.Trim();
                if (string.IsNullOrEmpty(name) || map.ContainsKey(name)) continue;

                var properties = vch.Element("Properties");
                var unit = properties?.Element("Measure")?.Value?.Trim() ?? "";
                var outFormat = properties?.Element("OutFormat")?.Value?.Trim() ?? "";
                var frequency = properties?.Element("Freq")?.Value?.Trim() ?? "";
                int? decimals = int.TryParse(properties?.Element("Decimals")?.Value?.Trim(), out var d) ? d : null;

                map[name] = new VchChannelSettings(unit, decimals, outFormat, frequency);
            }
        }
        return map;
    }

    /// <summary>VCH-Dateien sind i.d.R. Windows-1252 (z.B. °C); erst UTF-8 versuchen, sonst Latin1.</summary>
    private static string ReadText(string path)
    {
        var bytes = File.ReadAllBytes(path);
        try { return new UTF8Encoding(false, throwOnInvalidBytes: true).GetString(bytes); }
        catch (DecoderFallbackException) { return Encoding.Latin1.GetString(bytes); }
    }
}
