using System.Text.Json;

namespace SENTINEL
{
    /// <summary>
    /// Konfiguration für SENTINEL.
    /// Wird aus sentinel-config.json geladen (Fallback: Standardwerte).
    /// </summary>
    public sealed class SentinelConfig
    {
        /// <summary>
        /// Überwachte Verzeichnisse — SENTINEL durchsucht diese rekursiv nach Run_*-Ordnern.
        /// Typisch: OneDrive-Sync-Zielordner, z.B. ["C:\MXTrackData"]
        /// </summary>
        public List<string> WatchPaths { get; set; } = [];

        /// <summary>
        /// Polling-Intervall in Sekunden.
        /// Empfehlung: 30s für OneDrive-Sync (nicht zu aggressiv, nicht zu langsam).
        /// </summary>
        public int PollingIntervalSeconds { get; set; } = 30;

        /// <summary>
        /// Anzahl aufeinanderfolgender stabiler Checks bevor download.done erzeugt wird.
        /// Bei 30s Intervall und Threshold=3: Run muss 90s lang unverändert sein.
        /// </summary>
        public int StabilityThreshold { get; set; } = 3;

        /// <summary>
        /// Minimale Dateigröße in Bytes für cableData.ztx.
        /// Dateien unter diesem Wert gelten als OneDrive-Platzhalter (0-Byte-Dateien).
        /// Standard: 1024 Bytes (1 KB).
        /// </summary>
        public long MinimumFileSizeBytes { get; set; } = 1024;

        /// <summary>
        /// Lädt die Konfiguration aus sentinel-config.json im Anwendungsverzeichnis.
        /// Falls keine Datei existiert oder ein Fehler auftritt, werden Standardwerte verwendet.
        /// </summary>
        public static SentinelConfig Load()
        {
            var configPath = Path.Combine(AppContext.BaseDirectory, "sentinel-config.json");

            if (!File.Exists(configPath))
            {
                Console.WriteLine("Keine sentinel-config.json gefunden, verwende Standardkonfiguration.");
                return new SentinelConfig();
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

                return JsonSerializer.Deserialize<SentinelConfig>(json, options) ?? new SentinelConfig();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden der Konfiguration: {ex.Message}");
                Console.WriteLine("Verwende Standardkonfiguration.");
                return new SentinelConfig();
            }
        }
    }
}
