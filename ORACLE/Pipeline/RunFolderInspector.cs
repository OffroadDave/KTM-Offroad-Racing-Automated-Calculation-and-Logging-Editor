namespace Ktm.Racing.Oracle
{
    /// <summary>A run folder that the WinTAX API does not recognize as a valid run, with diagnosis.</summary>
    /// <param name="RunPath">Path of the <c>Run_*</c> folder.</param>
    /// <param name="Reason">Readable cause (e.g. lap folder without <c>cableData.ztx</c>).</param>
    /// <param name="IsEmpty">
    /// <c>true</c> for an empty run (no lap folders, no lap in <c>RunLapHeader.xml</c> – logger without recording).
    /// Empty runs contain no measurement data; they are not an error and only receive the metadata.
    /// </param>
    public sealed record SkippedRun(string RunPath, string Reason, bool IsEmpty = false);

    /// <summary>
    /// Finds <c>Run_*</c> folders purely via System.IO and diagnoses why a folder is not a valid
    /// WinTAX4 run. This way, runs that <c>FindRuns</c> does not return are no longer silently
    /// skipped (without native dependency, testable).
    /// </summary>
    public static class RunFolderInspector
    {
        /// <summary>
        /// All <c>Run_*</c> folders under <paramref name="root"/> (the folder itself, if it is a run folder).
        /// Does not descend into run folders.
        /// </summary>
        public static IReadOnlyList<string> FindRunFolders(string root)
        {
            if (!Directory.Exists(root))
                return [];
            if (IsRunFolderName(root))
                return [Path.GetFullPath(root)];

            var result = new List<string>();
            var queue = new Queue<string>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                string[] children;
                try { children = Directory.GetDirectories(queue.Dequeue()); }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { continue; }

                foreach (var child in children)
                {
                    var name = Path.GetFileName(child);
                    if (IsRunFolderName(child))
                        result.Add(Path.GetFullPath(child));
                    else if (!name.StartsWith("Lap_", StringComparison.OrdinalIgnoreCase)
                             && !name.StartsWith("_ORACLE", StringComparison.OrdinalIgnoreCase))
                        queue.Enqueue(child);
                }
            }

            return result.OrderBy(p => p, StringComparer.OrdinalIgnoreCase).ToList();
        }

        /// <summary>
        /// The <c>Run_*</c> folders under <paramref name="root"/> that are not contained in <paramref name="validRuns"/>
        /// (result of <c>FindRuns</c>/<c>IsRunLocation</c>), each with diagnosis.
        /// </summary>
        public static IReadOnlyList<SkippedRun> FindSkipped(string root, IEnumerable<string> validRuns)
        {
            var valid = new HashSet<string>(validRuns.Select(Normalize), StringComparer.OrdinalIgnoreCase);
            return FindRunFolders(root)
                .Where(folder => !valid.Contains(Normalize(folder)))
                .Select(folder => IsEmptyRun(folder)
                    ? new SkippedRun(folder, "empty run – no lap data recorded (logger started without recording)", IsEmpty: true)
                    : new SkippedRun(folder, Diagnose(folder)))
                .ToList();
        }

        /// <summary><c>true</c> if the run has no lap folders and <c>RunLapHeader.xml</c> references no lap.</summary>
        public static bool IsEmptyRun(string runFolder) =>
            Directory.Exists(runFolder)
            && Directory.GetDirectories(runFolder, "Lap_*", SearchOption.TopDirectoryOnly).Length == 0
            && (RunRepair.ReferencedLaps(runFolder)?.Count ?? 0) == 0;

        /// <summary>Describes why a run folder cannot be processed.</summary>
        public static string Diagnose(string runFolder)
        {
            if (!Directory.Exists(runFolder))
                return "folder does not exist";

            var reasons = new List<string>();
            var laps = Directory.GetDirectories(runFolder, "Lap_*", SearchOption.TopDirectoryOnly);
            if (laps.Length == 0)
                reasons.Add("no Lap_* folders");

            var orphans = RunRepair.FindOrphanLaps(runFolder);
            if (orphans.Count > 0)
                reasons.Add($"{orphans.Count} orphaned lap folder(s) without data (repairable, RepairOrphanLapFolders)");

            var withoutData = laps.Where(l => !File.Exists(Path.Combine(l, "cableData.ztx"))).ToList();
            if (withoutData.Count > 0)
                reasons.Add($"{withoutData.Count} of {laps.Length} lap folder(s) without cableData.ztx (e.g. {Path.GetFileName(withoutData[0])})");

            var nested = laps.Count(l => CountOccurrences(Path.GetFileName(l), "_Def_") > 1 || Path.GetFileName(l).Contains("_App_", StringComparison.OrdinalIgnoreCase));
            if (nested > 0)
                reasons.Add($"{nested} lap folder(s) split several times (_Def_/_App_) – repair the lap split in WinTAX");

            foreach (var file in new[] { "Session.xml", "Car.xml", "RunLapHeader.xml" })
            {
                if (!File.Exists(Path.Combine(runFolder, file)))
                    reasons.Add($"{file} missing");
            }

            return reasons.Count == 0
                ? "not recognized as a WinTAX4 run (unknown reason – check with 'ORACLE validate')"
                : string.Join("; ", reasons);
        }

        private static bool IsRunFolderName(string path) =>
            Path.GetFileName(Path.TrimEndingDirectorySeparator(path)).StartsWith("Run_", StringComparison.OrdinalIgnoreCase);

        private static string Normalize(string path) =>
            Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));

        private static int CountOccurrences(string text, string value)
        {
            var count = 0;
            for (var index = text.IndexOf(value, StringComparison.OrdinalIgnoreCase); index >= 0;
                 index = text.IndexOf(value, index + value.Length, StringComparison.OrdinalIgnoreCase))
                count++;
            return count;
        }
    }
}
