using System.Globalization;
using System.Xml.Linq;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Liest die von ORACLE in <c>LapHeader.xml</c> geschriebenen GPS-Rundendistanzen (<c>LapDistance</c>) bereits
    /// verarbeiteter Runs eines Events. So kann die Track-Länge auch für kurze Runs (Warm-up, Bike-Check) bestimmt
    /// werden, wenn ein anderer Run desselben Events genug Runden hatte.
    /// </summary>
    public static class LapHistory
    {
        /// <summary>
        /// Der Event-Ordner (Name = <paramref name="eventName"/>) oberhalb von <paramref name="path"/>, oder <c>null</c>.
        /// </summary>
        public static string? FindEventFolder(string path, string eventName)
        {
            if (string.IsNullOrWhiteSpace(eventName))
                return null;
            for (var dir = new DirectoryInfo(path); dir is not null; dir = dir.Parent)
            {
                if (string.Equals(dir.Name, eventName, StringComparison.OrdinalIgnoreCase))
                    return dir.FullName;
            }
            return null;
        }

        /// <summary>
        /// Rundendistanzen je Run (Lap-Reihenfolge) aller von ORACLE verarbeiteten Runs (mit <c>oracle.status.json</c>
        /// im Run- oder Geräteordner) unter <paramref name="eventFolder"/> außer <paramref name="excludeRuns"/>.
        /// Werte ≤ 0 (WinTAX-Rohdaten ohne GPS-Distanz) werden ignoriert.
        /// </summary>
        public static IReadOnlyList<IReadOnlyList<double>> ReadProcessedLapDistances(string eventFolder, IEnumerable<string> excludeRuns)
        {
            var exclude = new HashSet<string>(excludeRuns.Select(r => Path.TrimEndingDirectorySeparator(Path.GetFullPath(r))), StringComparer.OrdinalIgnoreCase);
            var result = new List<IReadOnlyList<double>>();
            foreach (var run in RunFolderInspector.FindRunFolders(eventFolder))
            {
                if (exclude.Contains(Path.TrimEndingDirectorySeparator(run)) || !IsProcessedByOracle(run))
                    continue;

                var laps = new List<double>();
                foreach (var lap in Directory.GetDirectories(run, "Lap_*").OrderBy(l => l, StringComparer.OrdinalIgnoreCase))
                {
                    if (ReadLapDistance(Path.Combine(lap, "LapHeader.xml")) is { } distance && distance > 0)
                        laps.Add(distance);
                }
                if (laps.Count > 0)
                    result.Add(laps);
            }
            return result;
        }

        private static bool IsProcessedByOracle(string run) =>
            File.Exists(Path.Combine(run, "oracle.status.json"))
            || File.Exists(Path.Combine(Path.GetDirectoryName(run) ?? run, "oracle.status.json"));

        /// <summary>Die von ORACLE geschriebenen Rundendistanzen (&gt; 0) der angegebenen Runs.</summary>
        public static IReadOnlyList<IReadOnlyList<double>> ReadLapDistances(IEnumerable<string> runs)
        {
            var result = new List<IReadOnlyList<double>>();
            foreach (var run in runs.Where(Directory.Exists))
            {
                var laps = Directory.GetDirectories(run, "Lap_*").OrderBy(l => l, StringComparer.OrdinalIgnoreCase)
                    .Select(lap => ReadLapDistance(Path.Combine(lap, "LapHeader.xml")))
                    .Where(d => d is > 0)
                    .Select(d => d!.Value)
                    .ToList();
                if (laps.Count > 0)
                    result.Add(laps);
            }
            return result;
        }

        private static double? ReadLapDistance(string lapHeader)
        {
            if (!File.Exists(lapHeader))
                return null;
            try
            {
                var value = XDocument.Load(lapHeader).Root?.Element("LapDistance")?.Value;
                return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var meters) && double.IsFinite(meters)
                    ? meters
                    : null;
            }
            catch (Exception ex) when (ex is IOException or System.Xml.XmlException or UnauthorizedAccessException)
            {
                return null;
            }
        }
    }
}
