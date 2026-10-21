namespace SENTINEL
{
    /// <summary>
    /// Core component: watches configured directories, detects new runs
    /// and generates download.done as soon as a run is stable.
    /// 
    /// <para><b>Sequence per scan cycle:</b></para>
    /// <list type="number">
    ///   <item>Search all WatchPaths recursively for Run_* folders</item>
    ///   <item>Skip runs with an existing download.done</item>
    ///   <item>For remaining runs: create snapshot → feed StabilityTracker</item>
    ///   <item>Stable runs: generate download.done → remove from tracking</item>
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
        /// Performs a complete scan cycle.
        /// Called by the polling loop in Program.cs.
        /// </summary>
        /// <returns>Number of download.done files generated in this cycle</returns>
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
                            Console.WriteLine($"  ✅ download.done generated: {donePath}");
                            _tracker.Remove(runPath);
                            generatedCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ⚠️ Error for {runPath}: {ex.Message}");
                }
            }

            return generatedCount;
        }

        /// <summary>
        /// Searches all WatchPaths recursively for Run_* folders without download.done.
        /// Inaccessible subfolders are skipped instead of aborting the entire WatchPath.
        /// If <see cref="SentinelConfig.MaxRunAgeDays"/> &gt; 0, runs unchanged for longer are ignored.
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
                    Console.WriteLine($"  ⚠️ WatchPath does not exist: {watchPath}");
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
                    Console.WriteLine($"  ⚠️ Error while scanning {watchPath}: {ex.Message}");
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
        /// Most recent write time of the run folder and its lap folders (downloads create new laps
        /// without necessarily modifying the run folder itself).
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

        /// <summary>Returns the number of currently tracked (not yet stable) runs.</summary>
        public int TrackedRunCount => _tracker.TrackedRunCount;

        private static void LogSnapshotStatus(RunSnapshot snapshot)
        {
            var status = snapshot.IsStructurallyValid ? "valid" : "invalid";
            var issues = new List<string>();

            if (snapshot.HasZeroByteFiles) issues.Add("0-byte files");
            if (snapshot.HasPlaceholderFiles) issues.Add("cloud placeholders");
            if (snapshot.HasLockedFiles) issues.Add("locked files");
            if (snapshot.LapCount == 0) issues.Add("no laps");
            if (!snapshot.IsComplete) issues.Add($"{snapshot.MissingFiles.Count} missing file(s) according to RunLapHeader.xml");

            var issueText = issues.Count > 0 ? $" ({string.Join(", ", issues)})" : "";

            Console.WriteLine($"  📊 {Path.GetFileName(snapshot.RunPath)}: " +
                $"{snapshot.FileCount} files, {snapshot.TotalSizeBytes / 1024} KB, " +
                $"{snapshot.LapCount} Laps — {status}{issueText}");
        }
    }
}
