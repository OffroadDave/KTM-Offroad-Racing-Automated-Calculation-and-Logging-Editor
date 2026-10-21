using System.Xml.Linq;

namespace Ktm.Racing.Oracle
{
    /// <summary>Result of a run repair.</summary>
    /// <param name="RunPath">Run folder.</param>
    /// <param name="MovedLaps">Moved, orphaned lap folders (names).</param>
    public sealed record RunRepairResult(string RunPath, IReadOnlyList<string> MovedLaps);

    /// <summary>
    /// Repairs runs that WinTAX does not recognize as valid because of orphaned lap folders: when splitting/appending laps
    /// (<c>_Def_</c>/<c>_App_</c>), the original lap folder is sometimes left behind without <c>cableData.ztx</c>. WinTAX
    /// no longer references it in <c>RunLapHeader.xml</c>, but discards the whole run.
    /// <para>Only what has <b>no data</b> (<c>cableData.ztx</c> missing) <b>and</b> is not referenced in <c>RunLapHeader.xml</c>
    /// is moved – and only if the run has at least one referenced lap with data. The target is
    /// <c>&lt;Run&gt;\_ORACLE_removed\</c> (reversible, nothing is deleted).</para>
    /// </summary>
    public static class RunRepair
    {
        /// <summary>Subfolder in the run into which orphaned lap folders are moved.</summary>
        public const string RemovedFolderName = "_ORACLE_removed";

        /// <summary>The orphaned lap folders of a run that can safely be moved (empty if none).</summary>
        public static IReadOnlyList<string> FindOrphanLaps(string runFolder)
        {
            if (!Directory.Exists(runFolder))
                return [];

            var referenced = ReferencedLaps(runFolder);
            if (referenced is null || referenced.Count == 0)
                return [];

            var laps = Directory.GetDirectories(runFolder, "Lap_*", SearchOption.TopDirectoryOnly);
            var hasData = laps.Any(l => referenced.Contains(Path.GetFileName(l)) && File.Exists(Path.Combine(l, "cableData.ztx")));
            if (!hasData)
                return [];

            return laps
                .Where(l => !File.Exists(Path.Combine(l, "cableData.ztx")) && !referenced.Contains(Path.GetFileName(l)))
                .OrderBy(l => l, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        /// <summary>Moves the orphaned lap folders of a run; <c>null</c> if there is nothing to do.</summary>
        public static RunRepairResult? Repair(string runFolder)
        {
            var orphans = FindOrphanLaps(runFolder);
            if (orphans.Count == 0)
                return null;

            var target = Path.Combine(runFolder, RemovedFolderName);
            Directory.CreateDirectory(target);
            var moved = new List<string>();
            foreach (var lap in orphans)
            {
                var name = Path.GetFileName(lap);
                var destination = Path.Combine(target, name);
                for (var i = 1; Directory.Exists(destination); i++)
                    destination = Path.Combine(target, $"{name}.{i}");
                try
                {
                    Directory.Move(lap, destination);
                    moved.Add(name);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    Console.WriteLine($"[repair] ⚠️ {name} could not be moved: {ex.Message}");
                }
            }

            if (moved.Count > 0)
            {
                File.AppendAllText(Path.Combine(target, "README.txt"),
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} ORACLE moved {moved.Count} empty lap folder(s) without cableData.ztx that are not " +
                    $"referenced in RunLapHeader.xml (leftovers of a WinTAX lap split/append): {string.Join(", ", moved)}.{Environment.NewLine}" +
                    $"To undo, move them back into the run folder.{Environment.NewLine}");
            }
            return moved.Count == 0 ? null : new RunRepairResult(runFolder, moved);
        }

        /// <summary>Repairs all runs under <paramref name="root"/> (incl. <paramref name="root"/> itself, if it is a run).</summary>
        public static IReadOnlyList<RunRepairResult> RepairAll(string root)
        {
            var results = new List<RunRepairResult>();
            foreach (var run in RunFolderInspector.FindRunFolders(root))
            {
                if (Repair(run) is { } result)
                {
                    Console.WriteLine($"[repair] {Path.GetFileName(run)}: moved {result.MovedLaps.Count} orphaned lap folder(s) without data to " +
                                      $"{RemovedFolderName} ({string.Join(", ", result.MovedLaps)}).");
                    results.Add(result);
                }
            }
            return results;
        }

        /// <summary>The lap folders referenced in <c>RunLapHeader.xml</c>; <c>null</c> if the file is missing/invalid.</summary>
        public static HashSet<string>? ReferencedLaps(string runFolder)
        {
            var file = Path.Combine(runFolder, "RunLapHeader.xml");
            if (!File.Exists(file))
                return null;
            try
            {
                return XDocument.Load(file).Descendants("LapInfo")
                    .Select(info => info.Element("Path")?.Value ?? string.Empty)
                    .Select(path => path.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty)
                    .Where(name => name.Length > 0)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex) when (ex is IOException or System.Xml.XmlException or UnauthorizedAccessException)
            {
                return null;
            }
        }
    }
}
