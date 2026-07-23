namespace SENTINEL
{
    /// <summary>
    /// SENTINEL — Download-Completion-Monitor für KRAKEN.
    /// 
    /// Überwacht OneDrive-synchronisierte Ordner und generiert download.done Dateien
    /// sobald ein Run_*-Ordner stabil ist (= OneDrive-Sync abgeschlossen).
    /// 
    /// <para><b>Stabilitäts-Algorithmus:</b></para>
    /// Scannt Run-Ordner in konfigurierbaren Intervallen, erstellt Snapshots
    /// (Dateianzahl + Gesamtgröße + Lock-/Platzhalterprüfung) und vergleicht
    /// aufeinanderfolgende Snapshots. Nach N stabilen Zyklen (Standard: 3 × 30s = 90s)
    /// wird download.done generiert, was KRAKEN die Job-Pipeline starten lässt.
    /// 
    /// <para><b>Konfiguration:</b> sentinel-config.json im Anwendungsverzeichnis</para>
    /// <para><b>Beenden:</b> Ctrl+C</para>
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

            // --- Konfiguration laden ---
            var config = SentinelConfig.Load();

            if (config.WatchPaths.Count == 0)
            {
                Console.WriteLine("❌ Keine WatchPaths konfiguriert. Bitte sentinel-config.json prüfen.");
                return 1;
            }

            Console.WriteLine($"Konfiguration:");
            Console.WriteLine($"  Polling-Intervall:     {config.PollingIntervalSeconds}s");
            Console.WriteLine($"  Stabilitäts-Schwelle:  {config.StabilityThreshold} Zyklen");
            Console.WriteLine($"  Min. Dateigröße (ZTX): {config.MinimumFileSizeBytes} Bytes");
            Console.WriteLine($"  Überwachte Pfade:");
            foreach (var path in config.WatchPaths)
            {
                var exists = Directory.Exists(path) ? "✅" : "❌ nicht gefunden";
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
                Console.WriteLine("⏹ Beende SENTINEL...");
            };

            // --- Polling-Loop ---
            var watcher = new RunWatcher(config);
            int totalGenerated = 0;
            int cycleNumber = 0;

            Console.WriteLine("🔍 Starte Überwachung... (Ctrl+C zum Beenden)");
            Console.WriteLine(new string('─', 60));

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    cycleNumber++;
                    Console.WriteLine($"[Zyklus {cycleNumber}] {DateTime.Now:HH:mm:ss} — Scanne...");

                    var generated = watcher.RunScanCycle();
                    totalGenerated += generated;

                    var pending = watcher.TrackedRunCount;
                    if (pending > 0)
                    {
                        Console.WriteLine($"  ⏳ {pending} Run(s) werden überwacht");
                    }

                    if (generated > 0)
                    {
                        Console.WriteLine($"  🎯 {generated} download.done generiert (gesamt: {totalGenerated})");
                    }

                    Console.WriteLine();
                    await Task.Delay(TimeSpan.FromSeconds(config.PollingIntervalSeconds), cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // Ctrl+C — normales Beenden
            }

            Console.WriteLine(new string('─', 60));
            Console.WriteLine($"SENTINEL beendet. {totalGenerated} download.done Datei(en) generiert.");
            return 0;
        }
    }
}
