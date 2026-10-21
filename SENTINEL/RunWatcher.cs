namespace SENTINEL
{
    /// <summary>
    /// Kernkomponente: Überwacht konfigurierte Verzeichnisse, erkennt neue Runs
    /// und generiert download.done sobald ein Run stabil ist.
    /// 
    /// <para><b>Ablauf pro Scan-Zyklus:</b></para>
    /// <list type="number">
    ///   <item>Alle WatchPaths rekursiv nach Run_*-Ordnern durchsuchen</item>
    ///   <item>Runs mit existierender download.done überspringen</item>
    ///   <item>Für verbleibende Runs: Snapshot erstellen → StabilityTracker füttern</item>
    ///   <item>Stabile Runs: download.done generieren → aus Tracking entfernen</item>
    /// </list>
    /// </summary>
    public sealed class RunWatcher
    {
        private readonly SentinelConfig _config;
        private readonly StabilityTracker _tracker;

        public RunWatcher(SentinelConfig config)
        {
            _config = config;
            _tracker = new StabilityTracker(config.StabilityThreshold);
        }

        /// <summary>
        /// Führt einen kompletten Scan-Zyklus durch.
        /// Wird vom Polling-Loop in Program.cs aufgerufen.
        /// </summary>
        /// <returns>Anzahl der in diesem Zyklus generierten download.done Dateien</returns>
        public int RunScanCycle()
        {
            var pendingRuns = DiscoverPendingRuns();
            _tracker.RetainOnly(pendingRuns);
            int generatedCount = 0;

            foreach (var runPath in pendingRuns)
            {
                try
                {
                    var snapshot = RunSnapshot.Create(runPath, _config.MinimumFileSizeBytes);
                    _tracker.RecordSnapshot(snapshot);

                    LogSnapshotStatus(snapshot);

                    if (_tracker.IsStable(runPath))
                    {
                        var latestSnapshot = _tracker.GetLatestSnapshot(runPath);
                        if (latestSnapshot is not null)
                        {
                            var donePath = DownloadDoneWriter.Write(latestSnapshot);
                            Console.WriteLine($"  ✅ download.done generiert: {donePath}");
                            _tracker.Remove(runPath);
                            generatedCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ⚠️ Fehler bei {runPath}: {ex.Message}");
                }
            }

            return generatedCount;
        }

        /// <summary>
        /// Durchsucht alle WatchPaths rekursiv nach Run_*-Ordnern ohne download.done.
        /// Unzugängliche Unterordner werden übersprungen statt den ganzen WatchPath abzubrechen.
        /// Bei <see cref="SentinelConfig.MaxRunAgeDays"/> &gt; 0 werden länger unveränderte Runs ignoriert.
        /// </summary>
        public List<string> DiscoverPendingRuns()
        {
            var pendingRuns = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            DateTime? cutoffUtc = _config.MaxRunAgeDays > 0
                ? DateTime.UtcNow.AddDays(-_config.MaxRunAgeDays)
                : null;

            foreach (var watchPath in _config.WatchPaths)
            {
                if (!Directory.Exists(watchPath))
                {
                    Console.WriteLine($"  ⚠️ WatchPath existiert nicht: {watchPath}");
                    continue;
                }

                try
                {
                    var runDirs = Directory.EnumerateDirectories(watchPath, "Run_*", DirectoryEnumeration);

                    foreach (var runDir in runDirs)
                    {
                        if (!seen.Add(Path.GetFullPath(runDir)))
                            continue;
                        if (DownloadDoneWriter.Exists(runDir))
                            continue;
                        if (cutoffUtc is not null && LastActivityUtc(runDir) < cutoffUtc)
                            continue;

                        pendingRuns.Add(runDir);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ⚠️ Fehler beim Scannen von {watchPath}: {ex.Message}");
                }
            }

            return pendingRuns;
        }

        private static readonly EnumerationOptions DirectoryEnumeration = new()
        {
            IgnoreInaccessible = true,
            RecurseSubdirectories = true,
        };

        /// <summary>
        /// Jüngster Schreibzeitpunkt des Run-Ordners und seiner Lap-Ordner (Downloads legen neue Laps an,
        /// ohne zwingend den Run-Ordner selbst zu ändern).
        /// </summary>
        private static DateTime LastActivityUtc(string runDir)
        {
            var latest = Directory.GetLastWriteTimeUtc(runDir);
            foreach (var lap in Directory.EnumerateDirectories(runDir, "Lap_*", new EnumerationOptions { IgnoreInaccessible = true }))
            {
                var time = Directory.GetLastWriteTimeUtc(lap);
                if (time > latest) latest = time;
            }
            return latest;
        }

        /// <summary>Gibt die Anzahl der aktuell getrackten (noch instabilen) Runs zurück.</summary>
        public int TrackedRunCount => _tracker.TrackedRunCount;

        private static void LogSnapshotStatus(RunSnapshot snapshot)
        {
            var status = snapshot.IsStructurallyValid ? "gültig" : "ungültig";
            var issues = new List<string>();

            if (snapshot.HasZeroByteFiles) issues.Add("0-Byte-Dateien");
            if (snapshot.HasPlaceholderFiles) issues.Add("Cloud-Platzhalter");
            if (snapshot.HasLockedFiles) issues.Add("gesperrte Dateien");
            if (snapshot.LapCount == 0) issues.Add("keine Laps");
            if (!snapshot.IsComplete) issues.Add($"{snapshot.MissingFiles.Count} fehlende Datei(en) laut RunLapHeader.xml");

            var issueText = issues.Count > 0 ? $" ({string.Join(", ", issues)})" : "";

            Console.WriteLine($"  📊 {Path.GetFileName(snapshot.RunPath)}: " +
                $"{snapshot.FileCount} Dateien, {snapshot.TotalSizeBytes / 1024} KB, " +
                $"{snapshot.LapCount} Laps — {status}{issueText}");
        }
    }
}
