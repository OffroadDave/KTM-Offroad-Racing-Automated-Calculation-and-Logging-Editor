using System.Globalization;
using System.Xml.Linq;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Writes run/lap properties DIRECTLY into the WinTAX4 XML files.
    ///
    /// <para>Replaces the former API-based variant: the KtmDataHandler property API
    /// (<c>AddRunProperty</c>/<c>AddLapProperty</c>) does not persist in this environment to the
    /// XML files that WinTAX4/downstream actually read. The XmlRunEditor therefore edits:</para>
    /// <list type="bullet">
    ///   <item><b>Primarily</b> the <c>LapHeader.xml</c> per lap: Lap, TrackLap, Alias, Run, TrackRun, Marker, LapDistance</item>
    ///   <item><c>Car.xml</c>: Name, DriverName</item>
    ///   <item><c>Session.xml</c>: Track, TrackLen</item>
    ///   <item><b>Secondarily</b> the aggregated <c>RunLapHeader.xml</c> (infos per lap), consistently</item>
    /// </list>
    /// </summary>
    public class XmlRunEditor
    {
        /// <summary>
        /// Sets all run and lap properties directly in the XML files of a run folder.
        /// </summary>
        /// <param name="lapOffset">Basis for continuous lap numbering across several race runs (option C).</param>
        /// <param name="isLastInStint">True if this is the last race run (only then a box marker at the end).</param>
        /// <param name="lapDistances">
        /// GPS distance per lap [m] (index 0 = first lap); null = leave LapDistance unchanged. An unknown value
        /// (<see cref="double.NaN"/> or missing entry) removes an existing (stale) LapDistance.
        /// </param>
        /// <returns>Number of laps in this run (for the offset accumulation).</returns>
        /// <exception cref="Exception">An XML file cannot be read/written – the caller counts the run as failed.</exception>
        public int SetRunProperties(string runPath, string alias, string track, string trackLength, string rider,
            int lapOffset = 0, bool isLastInStint = true, IReadOnlyList<double>? lapDistances = null)
        {
            // Normalize alias defensively: guarantees no spaces in XML/HED (e.g. "AA 01" -> "AA01").
            alias = BikeAlias.Normalize(alias);

            string runFolderName = new DirectoryInfo(runPath).Name;
            int runNumber = int.TryParse(runFolderName.Replace("Run_", ""), out int num) ? num : 0;

            // Lap order as in the data (RunLapHeader.xml, only laps with data) – the same order as lapDistances.
            var lapDirs = OrderedLapFolders(runPath);
            int numLaps = lapDirs.Count;

            // --- Run level (Car.xml / Session.xml) ---
            UpdateCarXml(runPath, alias, rider);
            UpdateSessionXml(runPath, track, trackLength);

            // --- per-lap LapHeader.xml (PRIMARY) ---
            for (int i = 0; i < numLaps; i++)
            {
                int continuousLap = lapOffset + i + 1;
                bool lastLapOfStint = isLastInStint && i == numLaps - 1;
                double? lapDist = LapDistanceAt(lapDistances, i);
                UpdateLapHeaderXml(lapDirs[i], continuousLap, alias, runNumber,
                    lastLapOfStint ? "Box" : "Lap", lapDist);
            }

            // --- RunLapHeader.xml (SECONDARY, consistent) ---
            UpdateRunLapHeaderXml(runPath, lapDirs, alias, runNumber, lapOffset, isLastInStint, lapDistances);

            Console.WriteLine(
                $"XML properties set for {runFolderName}: {numLaps} lap(s), " +
                $"lap {lapOffset + 1}..{lapOffset + numLaps}, Alias=\"{alias}\", Run={runNumber}.");

            return numLaps;
        }

        /// <summary>
        /// The lap folders of a run in data order (<c>RunLapHeader.xml</c>, only folders with data); without
        /// RunLapHeader all <c>Lap_*</c> folders by name.
        /// </summary>
        public static List<string> OrderedLapFolders(string runPath)
        {
            var ordered = LapSplitStore.ReadLapFolders(runPath).Select(f => Path.Combine(runPath, f.Folder)).ToList();
            return ordered.Count > 0
                ? ordered
                : Directory.GetDirectories(runPath, "Lap_*", SearchOption.TopDirectoryOnly)
                    .OrderBy(p => Path.GetFileName(p), StringComparer.OrdinalIgnoreCase)
                    .ToList();
        }

        // =====================================================================
        //  XML editors per file
        // =====================================================================

        private static void UpdateCarXml(string runPath, string name, string driver)
        {
            var file = Path.Combine(runPath, "Car.xml");
            if (!File.Exists(file)) return;

            var doc = XDocument.Load(file);
            if (doc.Root is not { } car) return;
            SetChild(car, "Name", name);
            SetChild(car, "DriverName", driver);
            AtomicFile.Save(doc, file);
        }

        private static void UpdateSessionXml(string runPath, string track, string trackLength)
        {
            var file = Path.Combine(runPath, "Session.xml");
            if (!File.Exists(file)) return;

            var doc = XDocument.Load(file);
            if (doc.Root is not { } session) return;
            SetChild(session, "Track", track);
            SetChild(session, "TrackLen", trackLength);
            AtomicFile.Save(doc, file);
        }

        /// <summary>
        /// Sets only <c>TrackLen</c> in the <c>Session.xml</c> of a run (after the track length check,
        /// when the length was only determined from the lap distances). Non-fatal.
        /// </summary>
        public void SetTrackLength(string runPath, string trackLength)
        {
            try
            {
                var file = Path.Combine(runPath, "Session.xml");
                if (!File.Exists(file)) return;

                var doc = XDocument.Load(file);
                if (doc.Root is not { } session) return;
                SetChild(session, "TrackLen", trackLength);
                AtomicFile.Save(doc, file);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting TrackLen for {Path.GetFileName(runPath)}: {ex.Message}");
            }
        }

        private static void UpdateLapHeaderXml(string lapDir, int lapNumber, string alias, int runNumber,
            string marker, double? lapDistance)
        {
            var file = Path.Combine(lapDir, "LapHeader.xml");
            if (!File.Exists(file)) return;

            var doc = XDocument.Load(file);
            if (doc.Root is not { } lh) return;
            SetChild(lh, "Lap", lapNumber);
            SetChild(lh, "TrackLap", lapNumber);
            SetChild(lh, "Alias", alias);
            SetChild(lh, "Run", runNumber);
            SetChild(lh, "TrackRun", runNumber);
            SetChild(lh, "Marker", marker);
            if (lapDistance is double dist)
            {
                if (double.IsNaN(dist))
                    lh.Elements("LapDistance").Remove();
                else
                    SetChild(lh, "LapDistance", Math.Round(dist, 2));
            }

            // Normalize the alias segment in any <Path> fields ("\AA 01\" → "\AA01\").
            foreach (var pathEl in lh.Descendants("Path"))
                pathEl.Value = NormalizeAliasInPath(pathEl.Value, alias);

            AtomicFile.Save(doc, file);
        }

        private static void UpdateRunLapHeaderXml(string runPath, List<string> lapDirs, string alias,
            int runNumber, int lapOffset, bool isLastInStint, IReadOnlyList<double>? lapDistances)
        {
            var file = Path.Combine(runPath, "RunLapHeader.xml");
            if (!File.Exists(file)) return;

            // Folder name → (continuous lap no., marker, distance) for matching via the path.
            var byFolder = new Dictionary<string, (int Lap, string Marker, double? Dist)>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < lapDirs.Count; i++)
            {
                string folder = Path.GetFileName(lapDirs[i]);
                bool last = isLastInStint && i == lapDirs.Count - 1;
                double? dist = LapDistanceAt(lapDistances, i);
                byFolder[folder] = (lapOffset + i + 1, last ? "Box" : "Lap", dist);
            }

            var doc = XDocument.Load(file);
            var lapNodes = doc.Root?.Element("Run")?.Element("LapArray")?.Elements("Lap");
            if (lapNodes is null) return;

            foreach (var lapNode in lapNodes)
            {
                var path = lapNode.Element("LapInfo")?.Element("Path")?.Value ?? "";
                var folder = path.Split('\\', '/').FirstOrDefault(s => s.StartsWith("Lap_", StringComparison.OrdinalIgnoreCase)) ?? "";
                if (!byFolder.TryGetValue(folder, out var info)) continue;

                var infos = lapNode.Element("Infos");
                if (infos is null) continue;
                SetInfo(infos, "Lap", info.Lap);
                SetInfo(infos, "TrackLap", info.Lap);
                SetInfo(infos, "Alias", alias);
                SetInfo(infos, "CarName", alias);
                SetInfo(infos, "Run", runNumber);
                SetInfo(infos, "Marker", info.Marker);
                if (info.Dist is double dist)
                {
                    if (double.IsNaN(dist))
                        infos.Elements("Info").Where(e => (string?)e.Element("Name") == "LapDistance").Remove();
                    else
                        SetInfo(infos, "LapDistance", Math.Round(dist, 2));
                }
            }

            // Normalize the alias segment in ALL <Path> fields (especially Inf/Path as absolute
            // run path: "…\AA 01\Run_1083\" → "…\AA01\Run_1083\"). Relative lap paths remain untouched.
            foreach (var pathEl in doc.Descendants("Path"))
                pathEl.Value = NormalizeAliasInPath(pathEl.Value, alias);

            AtomicFile.Save(doc, file);
        }

        // =====================================================================
        //  Helper methods
        // =====================================================================

        /// <summary>
        /// Distance for lap <paramref name="index"/>: null = no distances passed (LapDistance unchanged),
        /// <see cref="double.NaN"/> = calculated but unknown (remove existing LapDistance).
        /// </summary>
        private static double? LapDistanceAt(IReadOnlyList<double>? lapDistances, int index)
            => lapDistances is null ? null : index < lapDistances.Count ? lapDistances[index] : double.NaN;

        /// <summary>Sets the value of a child element; creates it if it is missing.</summary>
        private static void SetChild(XElement parent, string name, object value)
        {
            var el = parent.Element(name);
            if (el is null)
            {
                el = new XElement(name);
                parent.Add(el);
            }
            el.Value = ToInvariant(value);
        }

        /// <summary>Sets a Name/Data info pair in a RunLapHeader &lt;Infos&gt; block; creates it if it is missing.</summary>
        private static void SetInfo(XElement infos, string name, object value)
        {
            var info = infos.Elements("Info").FirstOrDefault(e => (string?)e.Element("Name") == name);
            if (info is null)
            {
                info = new XElement("Info", new XElement("Name", name), new XElement("Data"), new XElement("Infos"));
                infos.Add(info);
            }
            var data = info.Element("Data");
            if (data is null)
            {
                data = new XElement("Data");
                info.Add(data);
            }
            data.Value = ToInvariant(value);
        }

        private static string ToInvariant(object value)
            => value is IFormattable f ? f.ToString(null, CultureInfo.InvariantCulture) : value?.ToString() ?? "";

        /// <summary>
        /// Normalizes the alias segment in a path string: a path segment that (without whitespace)
        /// matches the canonical alias is replaced by the canonical alias
        /// (e.g. <c>"…\AA 01\Run_1083\"</c> → <c>"…\AA01\Run_1083\"</c>). Other segments remain untouched.
        /// </summary>
        private static string NormalizeAliasInPath(string path, string canonicalAlias)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(canonicalAlias)) return path;
            var segments = path.Split('\\');
            bool changed = false;
            for (int i = 0; i < segments.Length; i++)
            {
                if (!string.Equals(segments[i], canonicalAlias, StringComparison.Ordinal)
                    && string.Equals(BikeAlias.Normalize(segments[i]), canonicalAlias, StringComparison.Ordinal))
                {
                    segments[i] = canonicalAlias;
                    changed = true;
                }
            }
            return changed ? string.Join("\\", segments) : path;
        }
    }
}
