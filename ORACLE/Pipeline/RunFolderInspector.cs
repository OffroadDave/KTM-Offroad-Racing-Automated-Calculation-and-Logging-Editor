namespace Ktm.Racing.Oracle
{
    /// <summary>Ein Run-Ordner, den die WinTAX-API nicht als gültigen Run erkennt, mit Diagnose.</summary>
    /// <param name="RunPath">Pfad des <c>Run_*</c>-Ordners.</param>
    /// <param name="Reason">Lesbare Ursache (z.B. Lap-Ordner ohne <c>cableData.ztx</c>).</param>
    /// <param name="IsEmpty">
    /// <c>true</c> für einen leeren Run (keine Lap-Ordner, keine Lap in <c>RunLapHeader.xml</c> – Logger ohne Aufzeichnung).
    /// Leere Runs enthalten keine Messdaten; sie sind kein Fehler und erhalten nur die Metadaten.
    /// </param>
    public sealed record SkippedRun(string RunPath, string Reason, bool IsEmpty = false);

    /// <summary>
    /// Findet <c>Run_*</c>-Ordner rein per System.IO und diagnostiziert, warum ein Ordner kein gültiger
    /// WinTAX4-Run ist. Damit werden Runs, die <c>FindRuns</c> nicht liefert, nicht mehr stillschweigend
    /// übergangen (ohne native Abhängigkeit, testbar).
    /// </summary>
    public static class RunFolderInspector
    {
        /// <summary>
        /// Alle <c>Run_*</c>-Ordner unter <paramref name="root"/> (der Ordner selbst, falls er ein Run-Ordner ist).
        /// Steigt nicht in Run-Ordner ab.
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
        /// Die <c>Run_*</c>-Ordner unter <paramref name="root"/>, die nicht in <paramref name="validRuns"/>
        /// (Ergebnis von <c>FindRuns</c>/<c>IsRunLocation</c>) enthalten sind, jeweils mit Diagnose.
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

        /// <summary><c>true</c>, wenn der Run keine Lap-Ordner hat und <c>RunLapHeader.xml</c> keine Lap referenziert.</summary>
        public static bool IsEmptyRun(string runFolder) =>
            Directory.Exists(runFolder)
            && Directory.GetDirectories(runFolder, "Lap_*", SearchOption.TopDirectoryOnly).Length == 0
            && (RunRepair.ReferencedLaps(runFolder)?.Count ?? 0) == 0;

        /// <summary>Beschreibt, warum ein Run-Ordner nicht verarbeitet werden kann.</summary>
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
