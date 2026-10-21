namespace SENTINEL
{
    /// <summary>
    /// SENTINEL — download completion monitor for KRAKEN.
    /// 
    /// Watches OneDrive-synchronized folders and generates download.done files
    /// as soon as a Run_* folder is stable (= OneDrive sync completed).
    /// 
    /// <para><b>Stability algorithm:</b></para>
    /// Scans run folders at configurable intervals, creates snapshots
    /// (file count + total size + lock/placeholder check) and compares
    /// consecutive snapshots. After N stable cycles (default: 3 × 30s = 90s)
    /// download.done is generated, which lets KRAKEN start the job pipeline.
    /// 
    /// <para><b>Configuration:</b> sentinel-config.json in the application directory</para>
    /// <para><b>Exit:</b> Ctrl+C</para>
    /// </summary>
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║  SENTINEL — Download Completion Monitor     ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");
            Console.WriteLine();

            // --- Load configuration ---
            var config = SentinelConfig.Load();

            if (config.WatchPaths.Count == 0)
            {
                Console.WriteLine("❌ No WatchPaths configured. Please check sentinel-config.json.");
                return 1;
            }

            Console.WriteLine($"Configuration:");
            Console.WriteLine($"  Polling interval:      {config.PollingIntervalSeconds}s");
            Console.WriteLine($"  Stability threshold:   {config.StabilityThreshold} cycles");
            Console.WriteLine($"  Min. file size (ZTX):  {config.MinimumFileSizeBytes} bytes");
            Console.WriteLine($"  Watched paths:");
            foreach (var path in config.WatchPaths)
            {
                var exists = Directory.Exists(path) ? "✅" : "❌ not found";
                Console.WriteLine($"    → {path} {exists}");
            }
            Console.WriteLine();

            // --- Ctrl+C Handling ---
            using var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine();
                Console.WriteLine("⏹ Stopping SENTINEL...");
            };

            // --- Polling loop ---
            var watcher = new RunWatcher(config);
            int totalGenerated = 0;
            int cycleNumber = 0;

            Console.WriteLine("🔍 Starting monitoring... (Ctrl+C to exit)");
            Console.WriteLine(new string('─', 60));

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    cycleNumber++;
                    Console.WriteLine($"[Cycle {cycleNumber}] {DateTime.Now:HH:mm:ss} — Scanning...");

                    var generated = watcher.RunScanCycle();
                    totalGenerated += generated;

                    var pending = watcher.TrackedRunCount;
                    if (pending > 0)
                    {
                        Console.WriteLine($"  ⏳ {pending} run(s) being monitored");
                    }

                    if (generated > 0)
                    {
                        Console.WriteLine($"  🎯 {generated} download.done generated (total: {totalGenerated})");
                    }

                    Console.WriteLine();
                    await Task.Delay(TimeSpan.FromSeconds(config.PollingIntervalSeconds), cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // Ctrl+C — normal exit
            }

            Console.WriteLine(new string('─', 60));
            Console.WriteLine($"SENTINEL stopped. {totalGenerated} download.done file(s) generated.");
            return 0;
        }
    }
}
