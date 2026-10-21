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
        /// Runs, deren Ordner länger als diese Anzahl Tage nicht verändert wurde, werden ignoriert
        /// (verhindert, dass alte Runs ohne download.done bei jedem Zyklus erneut gescannt werden).
        /// 0 = deaktiviert (Standard, bisheriges Verhalten).
        /// </summary>
        public int MaxRunAgeDays { get; set; } = 0;

        /// <summary>Untergrenze für das Polling-Intervall in Sekunden.</summary>
        public const int MinPollingIntervalSeconds = 1;

        /// <summary>Obergrenze für das Polling-Intervall in Sekunden (1 Stunde).</summary>
        public const int MaxPollingIntervalSeconds = 3600;

        /// <summary>
        /// Lädt die Konfiguration aus sentinel-config.json im Anwendungsverzeichnis.
        /// Falls keine Datei existiert oder ein Fehler auftritt, werden Standardwerte verwendet.
        /// </summary>
        public static SentinelConfig Load() =>
            Load(Path.Combine(AppContext.BaseDirectory, "sentinel-config.json"));

        /// <summary>
        /// Lädt die Konfiguration aus der angegebenen Datei und korrigiert ungültige Werte (<see cref="Normalize"/>).
        /// Falls keine Datei existiert oder ein Fehler auftritt, werden Standardwerte verwendet.
        /// </summary>
        public static SentinelConfig Load(string configPath)
        {
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

                var config = JsonSerializer.Deserialize<SentinelConfig>(json, options) ?? new SentinelConfig();
                config.Normalize();
                return config;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden der Konfiguration: {ex.Message}");
                Console.WriteLine("Verwende Standardkonfiguration.");
                return new SentinelConfig();
            }
        }

        /// <summary>
        /// Korrigiert ungültige Werte: Ein Polling-Intervall von 0 oder negativ würde eine Dauerschleife bzw.
        /// eine Exception in <c>Task.Delay</c> erzeugen. Jede Korrektur wird auf der Konsole gemeldet.
        /// </summary>
        public void Normalize()
        {
            WatchPaths = (WatchPaths ?? [])
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var interval = Math.Clamp(PollingIntervalSeconds, MinPollingIntervalSeconds, MaxPollingIntervalSeconds);
            if (interval != PollingIntervalSeconds)
            {
                Console.WriteLine($"⚠️ pollingIntervalSeconds={PollingIntervalSeconds} ungültig, verwende {interval}s.");
                PollingIntervalSeconds = interval;
            }

            if (StabilityThreshold <= 0)
            {
                Console.WriteLine($"⚠️ stabilityThreshold={StabilityThreshold} ungültig, verwende 3.");
                StabilityThreshold = 3;
            }

            if (MinimumFileSizeBytes < 0)
            {
                Console.WriteLine($"⚠️ minimumFileSizeBytes={MinimumFileSizeBytes} ungültig, verwende 0.");
                MinimumFileSizeBytes = 0;
            }

            if (MaxRunAgeDays < 0)
            {
                Console.WriteLine($"⚠️ maxRunAgeDays={MaxRunAgeDays} ungültig, verwende 0 (deaktiviert).");
                MaxRunAgeDays = 0;
            }
        }
    }
}
