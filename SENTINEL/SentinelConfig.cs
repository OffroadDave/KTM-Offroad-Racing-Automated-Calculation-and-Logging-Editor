using System.Text.Json;

namespace SENTINEL
{
    /// <summary>
    /// Configuration for SENTINEL.
    /// Loaded from sentinel-config.json (fallback: default values).
    /// </summary>
    public sealed class SentinelConfig
    {
        /// <summary>
        /// Watched directories — SENTINEL searches them recursively for Run_* folders.
        /// Typically: OneDrive sync target folder, e.g. ["C:\MXTrackData"]
        /// </summary>
        public List<string> WatchPaths { get; set; } = [];

        /// <summary>
        /// Polling interval in seconds.
        /// Recommended: 30s for OneDrive sync (not too aggressive, not too slow).
        /// </summary>
        public int PollingIntervalSeconds { get; set; } = 30;

        /// <summary>
        /// Number of consecutive stable checks before download.done is created.
        /// With a 30s interval and Threshold=3: the run must remain unchanged for 90s.
        /// </summary>
        public int StabilityThreshold { get; set; } = 3;

        /// <summary>
        /// Minimum file size in bytes for cableData.ztx.
        /// Files below this value are considered OneDrive placeholders (0-byte files).
        /// Default: 1024 bytes (1 KB).
        /// </summary>
        public long MinimumFileSizeBytes { get; set; } = 1024;

        /// <summary>
        /// Runs whose folder has not been modified for more than this number of days are ignored
        /// (prevents old runs without download.done from being rescanned on every cycle).
        /// 0 = disabled (default, previous behavior).
        /// </summary>
        public int MaxRunAgeDays { get; set; } = 0;

        /// <summary>Lower bound for the polling interval in seconds.</summary>
        public const int MinPollingIntervalSeconds = 1;

        /// <summary>Upper bound for the polling interval in seconds (1 hour).</summary>
        public const int MaxPollingIntervalSeconds = 3600;

        /// <summary>
        /// Loads the configuration from sentinel-config.json in the application directory.
        /// If no file exists or an error occurs, default values are used.
        /// </summary>
        public static SentinelConfig Load() =>
            Load(Path.Combine(AppContext.BaseDirectory, "sentinel-config.json"));

        /// <summary>
        /// Loads the configuration from the given file and corrects invalid values (<see cref="Normalize"/>).
        /// If no file exists or an error occurs, default values are used.
        /// </summary>
        public static SentinelConfig Load(string configPath)
        {
            if (!File.Exists(configPath))
            {
                Console.WriteLine("No sentinel-config.json found, using default configuration.");
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
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                Console.WriteLine("Using default configuration.");
                return new SentinelConfig();
            }
        }

        /// <summary>
        /// Corrects invalid values: a polling interval of 0 or less would cause an endless loop or
        /// an exception in <c>Task.Delay</c>. Every correction is reported on the console.
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
                Console.WriteLine($"⚠️ pollingIntervalSeconds={PollingIntervalSeconds} invalid, using {interval}s.");
                PollingIntervalSeconds = interval;
            }

            if (StabilityThreshold <= 0)
            {
                Console.WriteLine($"⚠️ stabilityThreshold={StabilityThreshold} invalid, using 3.");
                StabilityThreshold = 3;
            }

            if (MinimumFileSizeBytes < 0)
            {
                Console.WriteLine($"⚠️ minimumFileSizeBytes={MinimumFileSizeBytes} invalid, using 0.");
                MinimumFileSizeBytes = 0;
            }

            if (MaxRunAgeDays < 0)
            {
                Console.WriteLine($"⚠️ maxRunAgeDays={MaxRunAgeDays} invalid, using 0 (disabled).");
                MaxRunAgeDays = 0;
            }
        }
    }
}
