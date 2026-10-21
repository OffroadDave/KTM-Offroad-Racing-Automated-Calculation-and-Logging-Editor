using System.Xml.Linq;

namespace Ktm.Racing.Oracle
{
    /// <summary>Ergebnis einer Run-Reparatur.</summary>
    /// <param name="RunPath">Run-Ordner.</param>
    /// <param name="MovedLaps">Verschobene, verwaiste Lap-Ordner (Namen).</param>
    public sealed record RunRepairResult(string RunPath, IReadOnlyList<string> MovedLaps);

    /// <summary>
    /// Repariert Runs, die WinTAX wegen verwaister Lap-Ordner nicht als gültig erkennt: Beim Teilen/Anhängen von Laps
    /// (<c>_Def_</c>/<c>_App_</c>) bleibt der ursprüngliche Lap-Ordner manchmal ohne <c>cableData.ztx</c> zurück. WinTAX
    /// referenziert ihn in <c>RunLapHeader.xml</c> nicht mehr, verwirft aber den ganzen Run.
    /// <para>Verschoben wird nur, was <b>keine Daten</b> hat (<c>cableData.ztx</c> fehlt) <b>und</b> in <c>RunLapHeader.xml</c>
    /// nicht referenziert ist – und nur, wenn der Run mindestens eine referenzierte Lap mit Daten besitzt. Ziel ist
    /// <c>&lt;Run&gt;\_ORACLE_removed\</c> (umkehrbar, nichts wird gelöscht).</para>
    /// </summary>
    public static class RunRepair
    {
        /// <summary>Unterordner im Run, in den verwaiste Lap-Ordner verschoben werden.</summary>
        public const string RemovedFolderName = "_ORACLE_removed";

        /// <summary>Die verwaisten Lap-Ordner eines Runs, die gefahrlos verschoben werden können (leer, wenn keine).</summary>
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

        /// <summary>Verschiebt die verwaisten Lap-Ordner eines Runs; <c>null</c>, wenn nichts zu tun ist.</summary>
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
                    Console.WriteLine($"[repair] ⚠️ {name} konnte nicht verschoben werden: {ex.Message}");
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

        /// <summary>Repariert alle Runs unter <paramref name="root"/> (inkl. <paramref name="root"/> selbst, falls ein Run).</summary>
        public static IReadOnlyList<RunRepairResult> RepairAll(string root)
        {
            var results = new List<RunRepairResult>();
            foreach (var run in RunFolderInspector.FindRunFolders(root))
            {
                if (Repair(run) is { } result)
                {
                    Console.WriteLine($"[repair] {Path.GetFileName(run)}: {result.MovedLaps.Count} verwaiste(r) Lap-Ordner ohne Daten nach " +
                                      $"{RemovedFolderName} verschoben ({string.Join(", ", result.MovedLaps)}).");
                    results.Add(result);
                }
            }
            return results;
        }

        /// <summary>Die in <c>RunLapHeader.xml</c> referenzierten Lap-Ordner; <c>null</c>, wenn die Datei fehlt/ungültig ist.</summary>
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
