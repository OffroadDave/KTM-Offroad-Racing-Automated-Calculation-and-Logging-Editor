using System.Text.Json;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Konfiguration für ORACLE.
    /// Wird aus oracle-config.json geladen (Fallback: Standardwerte).
    /// 
    /// Enthält konfigurierbare Pfade und Team-Mappings,
    /// die bisher als Konstanten im Code standen.
    /// </summary>
    public class OracleConfig
    {
        /// <summary>Verzeichnis der KOMPASS-Datenbank</summary>
        public string DatabaseDirectory { get; set; } = @"C:\KTM Tools\KTM Offroad Racing Shared Resources\SoftwareDB";

        /// <summary>Dateiname der KOMPASS-Datenbank</summary>
        public string DatabaseFileName { get; set; } = "KOMPASS.json";

        /// <summary>
        /// Mapping von Team-Ordner-Abkürzungen zu vollständigen Teamnamen.
        /// Wird beim KOMPASS-Fallback verwendet, wenn kein .HED-File vorhanden ist.
        /// </summary>
        public Dictionary<string, string> TeamMappings { get; set; } = new(StringComparer.OrdinalIgnoreCase)
        {
            ["01_KTM-FR"] = "KTM Factory Racing",
            ["KTM-FR"] = "KTM Factory Racing",
            ["02_HQV"] = "Husqvarna Factory Racing",
            ["HQV"] = "Husqvarna Factory Racing",
            ["03_KTM-DC"] = "KTM De Carli Racing",
            ["KTM-DC"] = "KTM De Carli Racing",
        };

        /// <summary>Schwellenwerte für die Renn-Erkennung (<see cref="RaceSegmentDetector"/>).</summary>
        public RaceDetectionThresholds RaceDetection { get; set; } = new();

        /// <summary>Vollständiger Pfad zur KOMPASS-Datenbankdatei</summary>
        public string DatabaseFilePath => Path.Combine(DatabaseDirectory, DatabaseFileName);

        /// <summary>
        /// Lädt die Konfiguration aus oracle-config.json im Anwendungsverzeichnis.
        /// Falls keine Datei existiert oder ein Fehler auftritt, werden Standardwerte verwendet.
        /// </summary>
        public static OracleConfig Load()
        {
            var configPath = Path.Combine(AppContext.BaseDirectory, "oracle-config.json");

            if (!File.Exists(configPath))
            {
                Console.WriteLine("Keine oracle-config.json gefunden, verwende Standardkonfiguration.");
                return new OracleConfig();
            }

            try
            {
                var json = File.ReadAllText(configPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };

                var config = JsonSerializer.Deserialize<OracleConfig>(json, options) ?? new OracleConfig();

                // JSON-Deserialisierung erzeugt Standard-Dictionary ohne custom Comparer.
                // Daher TeamMappings explizit mit case-insensitivem Comparer neu erstellen.
                config.TeamMappings = new Dictionary<string, string>(
                    config.TeamMappings, StringComparer.OrdinalIgnoreCase);

                Console.WriteLine("Konfiguration aus oracle-config.json geladen.");
                return config;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warnung: oracle-config.json konnte nicht geladen werden: {ex.Message}");
                Console.WriteLine("Verwende Standardkonfiguration.");
                return new OracleConfig();
            }
        }
    }
}
