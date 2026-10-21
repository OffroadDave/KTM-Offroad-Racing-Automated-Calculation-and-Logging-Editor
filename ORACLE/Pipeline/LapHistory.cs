using System.Globalization;
using System.Xml.Linq;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Reads the GPS lap distances (<c>LapDistance</c>) written by ORACLE to <c>LapHeader.xml</c> of already
    /// processed runs of an event. This allows the track length to be determined even for short runs (warm-up, bike check)
    /// if another run of the same event had enough laps.
    /// </summary>
    public static class LapHistory
    {
        /// <summary>
        /// The event folder (name = <paramref name="eventName"/>) above <paramref name="path"/>, or <c>null</c>.
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
        /// Lap distances per run (lap order) of all runs processed by ORACLE (with <c>oracle.status.json</c>
        /// in the run or device folder) under <paramref name="eventFolder"/> except <paramref name="excludeRuns"/>.
        /// Values ≤ 0 (WinTAX raw data without GPS distance) are ignored.
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

        /// <summary>The lap distances (&gt; 0) written by ORACLE for the specified runs.</summary>
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
