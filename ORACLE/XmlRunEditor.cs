using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ORACLE
{
    /// <summary>
    /// Service-Klasse für das Bearbeiten von XML-Dateien in Run-Ordnern.
    /// </summary>
    public class XmlRunEditor
    {
        /// <summary>
        /// Bearbeitet alle relevanten XML-Dateien für die angegebenen gültigen Runs.
        /// </summary>
        /// <param name="validRuns">Array mit Pfaden zu gültigen Run-Ordnern.</param>
        /// <param name="alias">Alias für Fahrer/Bike.</param>
        /// <param name="track">Track-Name.</param>
        /// <param name="trackLength">Track-Länge.</param>
        /// <param name="rider">Fahrername.</param>
        public void EditXMLFiles(string[] validRuns, string alias, string track, string trackLength, string rider)
        {
            for (int runIdx = 0; runIdx < validRuns.Length; runIdx++)
            {
                // Bearbeite LapHeader.xml
                EditLapXMLFiles(validRuns, runIdx, alias);
                // Bearbeite Session.xml
                EditSessionXML(validRuns, runIdx, track, trackLength);
                // Bearbeite Car.xml
                EditCarXML(validRuns, runIdx, alias, rider);
            }
        }

        /// <summary>
        /// Bearbeitet alle LapHeader.xml-Dateien im angegebenen Run-Ordner.
        /// </summary>
        /// <param name="validRuns">Array mit Pfaden zu gültigen Run-Ordnern.</param>
        /// <param name="runIdx">Index des aktuellen Runs.</param>
        /// <param name="alias">Alias für Fahrer/Bike.</param>
        public void EditLapXMLFiles(string[] validRuns, int runIdx, string alias)
        {
            Console.WriteLine($"Writing lapnumbers and alias \"{alias}\" in LapHeader.xml file.");
            var allLapFiles = GatherAllLapHeaders(validRuns[runIdx]);
            if (allLapFiles.Length == 0)
            {
                Console.WriteLine("No LapHeader.xml files found.");
                return;
            }
            int lapIdxTotal = allLapFiles.Length;
            for (int lapIdx = 0; lapIdx < lapIdxTotal; lapIdx++)
            {
                // Extrahiere Run-Nummer aus Ordnername
                string runFolderName = new DirectoryInfo(validRuns[runIdx]).Name;
                int runNumber = int.TryParse(runFolderName.Replace("Run_", ""), out int num) ? num : 0;
                // Bearbeite einzelne LapHeader.xml
                ModifyLapHeaderFile(allLapFiles[lapIdx], lapIdxTotal, lapIdx + 1, runNumber, alias);
            }
            Console.WriteLine($"Updated {lapIdxTotal} LapHeader.xml files with Alias \"{alias}\".");
        }

        /// <summary>
        /// Sammelt alle LapHeader.xml-Dateien in einem Run-Ordner, die neben cableData.ztx liegen.
        /// </summary>
        /// <param name="runFolder">Pfad zum Run-Ordner.</param>
        /// <returns>Array mit Pfaden zu LapHeader.xml-Dateien.</returns>
        public string[] GatherAllLapHeaders(string runFolder)
        {
            if (!Directory.Exists(runFolder))
                return Array.Empty<string>();
            var entries = Directory.GetFiles(runFolder, "cableData.ztx", SearchOption.AllDirectories);
            var lapFiles = new List<string>();
            foreach (var entry in entries)
            {
                var hdrPath = Path.Combine(Path.GetDirectoryName(entry) ?? "", "LapHeader.xml");
                if (File.Exists(hdrPath))
                    lapFiles.Add(hdrPath);
            }
            lapFiles.Sort();
            return lapFiles.ToArray();
        }

        /// <summary>
        /// Bearbeitet eine einzelne LapHeader.xml-Datei und setzt die relevanten Werte.
        /// </summary>
        /// <param name="filePath">Pfad zur LapHeader.xml-Datei.</param>
        /// <param name="numLapFiles">Gesamtanzahl der Laps.</param>
        /// <param name="lapNum">Aktuelle Lap-Nummer.</param>
        /// <param name="runNum">Run-Nummer.</param>
        /// <param name="aliasStr">Alias für Fahrer/Bike.</param>
        public void ModifyLapHeaderFile(string filePath, int numLapFiles, int lapNum, int runNum, string aliasStr)
        {
            Console.WriteLine($"Processing lap {lapNum} of {numLapFiles}.");
            try
            {
                var doc = new XmlDocument();
                doc.Load(filePath);
                var root = doc.DocumentElement;

                // Setze relevante XML-Tags
                UpdateNode(root, "Lap", lapNum.ToString());
                UpdateNode(root, "TrackLap", lapNum.ToString());
                UpdateNode(root, "Alias", aliasStr);
                UpdateNode(root, "Run", runNum.ToString());
                UpdateNode(root, "Marker", (lapNum >= 1 && lapNum < numLapFiles) ? "Lap" : "Box");

                doc.Save(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update LapHeader.xml: {ex.Message}");
            }
        }

        /// <summary>
        /// Bearbeitet die Session.xml-Datei im angegebenen Run-Ordner.
        /// </summary>
        /// <param name="validRuns">Array mit Pfaden zu gültigen Run-Ordnern.</param>
        /// <param name="runIdx">Index des aktuellen Runs.</param>
        /// <param name="track">Track-Name.</param>
        /// <param name="trackLen">Track-Länge.</param>
        public void EditSessionXML(string[] validRuns, int runIdx, string track, string trackLen)
        {
            Console.WriteLine($"Writing track \"{track}\" and trackLength \"{trackLen}\" m in Session.xml file.");
            string sessionFilePath = Path.Combine(validRuns[runIdx], "Session.xml");
            if (!File.Exists(sessionFilePath))
            {
                Console.WriteLine("Session.xml not found in run folder. Operation aborted.");
                return;
            }
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(sessionFilePath);
                var root = xmlDoc.DocumentElement;

                // Setze Track und TrackLen
                bool ok1 = UpdateNode(root, "Track", track);
                bool ok2 = UpdateNode(root, "TrackLen", trackLen);

                if (!ok1 || !ok2)
                {
                    Console.WriteLine("Missing <Track> or <TrackLen> tag in Session.xml. Update aborted.");
                    return;
                }

                xmlDoc.Save(sessionFilePath);
                Console.WriteLine("Session.xml successfully updated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to process Session.xml: {ex.Message}");
            }
        }

        /// <summary>
        /// Bearbeitet die Car.xml-Datei im angegebenen Run-Ordner.
        /// </summary>
        /// <param name="validRuns">Array mit Pfaden zu gültigen Run-Ordnern.</param>
        /// <param name="runIdx">Index des aktuellen Runs.</param>
        /// <param name="alias">Alias für Fahrer/Bike.</param>
        /// <param name="rider">Fahrername.</param>
        public void EditCarXML(string[] validRuns, int runIdx, string alias, string rider)
        {
            Console.WriteLine($"Writing alias \"{alias}\" and rider \"{rider}\" in Car.xml file.");
            string xmlFilePath = Path.Combine(validRuns[runIdx], "Car.xml");
            if (!File.Exists(xmlFilePath))
            {
                Console.WriteLine($"Car.xml does not exist at path: {xmlFilePath}");
                return;
            }
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFilePath);
                var root = xmlDoc.DocumentElement;

                // Setze Name und DriverName
                UpdateNode(root, "Name", alias);
                UpdateNode(root, "DriverName", rider);

                xmlDoc.Save(xmlFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error editing Car.xml: {ex.Message}");
            }
        }

        /// <summary>
        /// Hilfsmethode zum Setzen des Wertes eines XML-Tags.
        /// </summary>
        /// <param name="rootElem">Root-Element des XML-Dokuments.</param>
        /// <param name="tag">Tag-Name.</param>
        /// <param name="val">Wert, der gesetzt werden soll.</param>
        /// <returns>True, wenn erfolgreich; sonst false.</returns>
        public bool UpdateNode(XmlElement rootElem, string tag, string val)
        {
            var node = rootElem.GetElementsByTagName(tag).Cast<XmlNode>().FirstOrDefault();
            if (node == null)
                return false;
            try
            {
                node.InnerText = val;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}