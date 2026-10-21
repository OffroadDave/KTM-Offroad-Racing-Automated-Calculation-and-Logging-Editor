using System.Globalization;
using System.Xml.Linq;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Schreibt Run-/Lap-Properties DIREKT in die WinTAX4-XML-Dateien.
    ///
    /// <para>Ersetzt die frühere, API-basierte Variante: Die KtmDataHandler-Property-API
    /// (<c>AddRunProperty</c>/<c>AddLapProperty</c>) persistiert in dieser Umgebung nicht in die
    /// XML-Dateien, die WinTAX4/downstream tatsächlich lesen. Der XmlRunEditor bearbeitet daher:</para>
    /// <list type="bullet">
    ///   <item><b>Primär</b> je Lap die <c>LapHeader.xml</c>: Lap, TrackLap, Alias, Run, TrackRun, Marker, LapDistance</item>
    ///   <item><c>Car.xml</c>: Name, DriverName</item>
    ///   <item><c>Session.xml</c>: Track, TrackLen</item>
    ///   <item><b>Sekundär</b> die aggregierte <c>RunLapHeader.xml</c> (Infos je Lap) konsistent</item>
    /// </list>
    /// </summary>
    public class XmlRunEditor
    {
        /// <summary>
        /// Setzt alle Run- und Lap-Properties direkt in den XML-Dateien eines Run-Ordners.
        /// </summary>
        /// <param name="lapOffset">Basis für fortlaufende Lap-Nummerierung über mehrere Renn-Runs (Option C).</param>
        /// <param name="isLastInStint">True wenn dies der letzte Renn-Run ist (nur dann Box-Marker am Ende).</param>
        /// <param name="lapDistances">
        /// GPS-Distanz je Lap [m] (Index 0 = erste Lap); null = LapDistance unverändert lassen. Ein unbekannter Wert
        /// (<see cref="double.NaN"/> oder fehlender Eintrag) entfernt eine vorhandene (veraltete) LapDistance.
        /// </param>
        /// <returns>Anzahl der Laps in diesem Run (für die Offset-Akkumulation).</returns>
        /// <exception cref="Exception">Eine XML-Datei kann nicht gelesen/geschrieben werden – der Aufrufer zählt den Run als fehlgeschlagen.</exception>
        public int SetRunProperties(string runPath, string alias, string track, string trackLength, string rider,
            int lapOffset = 0, bool isLastInStint = true, IReadOnlyList<double>? lapDistances = null)
        {
            // Alias defensiv normalisieren: garantiert keine Leerzeichen in XML/HED (z.B. "AA 01" -> "AA01").
            alias = BikeAlias.Normalize(alias);

            string runFolderName = new DirectoryInfo(runPath).Name;
            int runNumber = int.TryParse(runFolderName.Replace("Run_", ""), out int num) ? num : 0;

            // Lap-Reihenfolge wie in den Daten (RunLapHeader.xml, nur Laps mit Daten) – dieselbe Reihenfolge wie lapDistances.
            var lapDirs = OrderedLapFolders(runPath);
            int numLaps = lapDirs.Count;

            // --- Run-Level (Car.xml / Session.xml) ---
            UpdateCarXml(runPath, alias, rider);
            UpdateSessionXml(runPath, track, trackLength);

            // --- per-Lap LapHeader.xml (PRIMÄR) ---
            for (int i = 0; i < numLaps; i++)
            {
                int continuousLap = lapOffset + i + 1;
                bool lastLapOfStint = isLastInStint && i == numLaps - 1;
                double? lapDist = LapDistanceAt(lapDistances, i);
                UpdateLapHeaderXml(lapDirs[i], continuousLap, alias, runNumber,
                    lastLapOfStint ? "Box" : "Lap", lapDist);
            }

            // --- RunLapHeader.xml (SEKUNDÄR, konsistent) ---
            UpdateRunLapHeaderXml(runPath, lapDirs, alias, runNumber, lapOffset, isLastInStint, lapDistances);

            Console.WriteLine(
                $"XML-Properties gesetzt für {runFolderName}: {numLaps} Lap(s), " +
                $"Lap {lapOffset + 1}..{lapOffset + numLaps}, Alias=\"{alias}\", Run={runNumber}.");

            return numLaps;
        }

        /// <summary>
        /// Die Lap-Ordner eines Runs in Daten-Reihenfolge (<c>RunLapHeader.xml</c>, nur Ordner mit Daten); ohne
        /// RunLapHeader alle <c>Lap_*</c>-Ordner nach Namen.
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
        //  XML-Editoren pro Datei
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
        /// Setzt nur <c>TrackLen</c> in der <c>Session.xml</c> eines Runs (nach der Track-Längen-Prüfung,
        /// wenn die Länge erst aus den Rundendistanzen ermittelt wurde). Non-fatal.
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
                Console.WriteLine($"Fehler beim Setzen von TrackLen für {Path.GetFileName(runPath)}: {ex.Message}");
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

            // Alias-Segment in etwaigen <Path>-Feldern normalisieren ("\AA 01\" → "\AA01\").
            foreach (var pathEl in lh.Descendants("Path"))
                pathEl.Value = NormalizeAliasInPath(pathEl.Value, alias);

            AtomicFile.Save(doc, file);
        }

        private static void UpdateRunLapHeaderXml(string runPath, List<string> lapDirs, string alias,
            int runNumber, int lapOffset, bool isLastInStint, IReadOnlyList<double>? lapDistances)
        {
            var file = Path.Combine(runPath, "RunLapHeader.xml");
            if (!File.Exists(file)) return;

            // Ordnername → (fortlaufende Lap-Nr, Marker, Distanz) für die Zuordnung über den Pfad.
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

            // Alias-Segment in ALLEN <Path>-Feldern normalisieren (v.a. Inf/Path als absoluter
            // Run-Pfad: "…\AA 01\Run_1083\" → "…\AA01\Run_1083\"). Relative Lap-Pfade bleiben unberührt.
            foreach (var pathEl in doc.Descendants("Path"))
                pathEl.Value = NormalizeAliasInPath(pathEl.Value, alias);

            AtomicFile.Save(doc, file);
        }

        // =====================================================================
        //  Hilfsmethoden
        // =====================================================================

        /// <summary>
        /// Distanz für Lap <paramref name="index"/>: null = keine Distanzen übergeben (LapDistance unverändert),
        /// <see cref="double.NaN"/> = berechnet, aber unbekannt (vorhandene LapDistance entfernen).
        /// </summary>
        private static double? LapDistanceAt(IReadOnlyList<double>? lapDistances, int index)
            => lapDistances is null ? null : index < lapDistances.Count ? lapDistances[index] : double.NaN;

        /// <summary>Setzt den Wert eines Kind-Elements; legt es an, falls es fehlt.</summary>
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

        /// <summary>Setzt ein Name/Data-Info-Paar in einem RunLapHeader-&lt;Infos&gt;-Block; legt es an, falls es fehlt.</summary>
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
        /// Normalisiert das Alias-Segment in einem Pfad-String: Ein Pfadsegment, das (ohne Leerraum)
        /// dem kanonischen Alias entspricht, wird durch den kanonischen Alias ersetzt
        /// (z.B. <c>"…\AA 01\Run_1083\"</c> → <c>"…\AA01\Run_1083\"</c>). Andere Segmente bleiben unberührt.
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
