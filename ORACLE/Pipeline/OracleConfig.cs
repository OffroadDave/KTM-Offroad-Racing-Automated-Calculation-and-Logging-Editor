using System.Text.Json;
using Ktm.Racing.Domain.Telemetry;

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

        /// <summary>Pfad zur MotoDB loadChannels.xlsx; dessen CIRCUITS/HED-Sheets werden aus KOMPASS synchronisiert (<c>sync-motodb</c>).</summary>
        public string MotoDbLoadChannelsPath { get; set; } = @"C:\KTM Tools\KTM MotoDB\loadChannels.xlsx";

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

        /// <summary>Schwellenwerte für den GPS-Lap-Split (<c>--gps-split</c>).</summary>
        public GpsLapSplitSettings GpsLapSplit { get; set; } = new();

        /// <summary>
        /// Wenn true, benennt ORACLE Geräteordner mit Leerzeichen im Namen kanonisch um
        /// (z.B. <c>"AA 01"</c> → <c>"AA01"</c>), bevor ein Rider-/Session-Ordner verarbeitet wird.
        /// So ist der bikeName in Ordnern und XML durchgängig ohne Leerzeichen.
        /// Non-fatal und guarded (nur nicht-kanonische Namen, nur wenn der Zielordner frei ist).
        /// Auf false setzen, falls nachgelagerte Tools (z.B. KRAKEN) die Originalordnernamen erwarten.
        /// </summary>
        public bool NormalizeDeviceFolderName { get; set; } = true;

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

    /// <summary>
    /// Konfigurierbare Schwellenwerte für den GPS-Lap-Split (<c>--gps-split</c>).
    /// Wird aus oracle-config.json (Abschnitt <c>GpsLapSplit</c>) geladen; sonst Defaults.
    /// </summary>
    public sealed class GpsLapSplitSettings
    {
        /// <summary>Korridor-Radius [m] um den Ziellinien-Punkt, der als Durchfahrt zählt.</summary>
        public double RadiusMeters { get; set; } = 25.0;

        /// <summary>Mindestzeit [s] zwischen zwei Lap-Grenzen (unterdrückt Doppel-Trigger).</summary>
        public double MinLapSeconds { get; set; } = 20.0;

        /// <summary>Wenn die Daten nach der letzten Durchfahrt um mind. eine Rundenzeit weiterlaufen, als Teil-Lap behalten.</summary>
        public bool IncludeTrailingPartialLap { get; set; } = true;

        /// <summary>Konvertiert die Einstellungen in die Domänen-Optionen des Splitters.</summary>
        public GpsLapSplitOptions ToOptions() => new()
        {
            RadiusMeters = RadiusMeters,
            MinLapSeconds = MinLapSeconds,
            IncludeTrailingPartialLap = IncludeTrailingPartialLap,
        };
    }
}
