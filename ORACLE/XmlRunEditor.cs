using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ORACLE
{
    /// <summary>
    /// [LEGACY FALLBACK] Service-Klasse für das direkte Bearbeiten von XML-Dateien in Run-Ordnern.
    /// 
    /// <para>
    /// <b>Hinweis:</b> Diese Klasse wurde durch <see cref="RunPropertyEditor"/> ersetzt,
    /// die die offizielle KtmDataHandler-API (AddRunProperty/AddLapProperty) verwendet.
    /// RunPropertyEditor ist die empfohlene Methode für das Setzen von Run-/Lap-Properties.
    /// </para>
    /// 
    /// <para>
    /// XmlRunEditor wird als Fallback-Implementierung beibehalten für Szenarien,
    /// in denen die KtmDataHandler-API nicht verfügbar ist oder direkte XML-Kontrolle
    /// über Session.xml, Car.xml und LapHeader.xml benötigt wird.
    /// </para>
    /// 
    /// <para><b>Bearbeitete XML-Dateien:</b></para>
    /// <list type="bullet">
    ///   <item>Session.xml ? Track, TrackLen (jetzt via AddRunProperty)</item>
    ///   <item>Car.xml ? Name (Alias), DriverName (jetzt via AddRunProperty)</item>
    ///   <item>LapHeader.xml ? Lap, TrackLap, Alias, Run, Marker (jetzt via AddLapProperty)</item>
    /// </list>
    /// </summary>
    [Obsolete("Ersetzt durch RunPropertyEditor. Beibehalten als Legacy-Fallback.")]
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
            foreach (var runPath in validRuns)
            {
                // Bearbeite LapHeader.xml
                EditLapXMLFiles(runPath, alias);
                // Bearbeite Session.xml
                EditSessionXML(runPath, track, trackLength);
                // Bearbeite Car.xml
                EditCarXML(runPath, alias, rider);
            }
        }

        /// <summary>
        /// Bearbeitet alle LapHeader.xml-Dateien im angegebenen Run-Ordner.
        /// </summary>
        /// <param name="runPath">Pfad zum Run-Ordner.</param>
        /// <param name="alias">Alias für Fahrer/Bike.</param>
        public void EditLapXMLFiles(string runPath, string alias)
        {
            Console.WriteLine($"Writing lapnumbers and alias \"{alias}\" in LapHeader.xml file.");
            var allLapFiles = GatherAllLapHeaders(runPath);
            if (allLapFiles.Length == 0)
            {
                Console.WriteLine("No LapHeader.xml files found.");
                return;
            }
            int lapIdxTotal = allLapFiles.Length;

            // Extrahiere Run-Nummer einmalig aus Ordnername
            string runFolderName = new DirectoryInfo(runPath).Name;
            int runNumber = int.TryParse(runFolderName.Replace("Run_", ""), out int num) ? num : 0;

            for (int lapIdx = 0; lapIdx < lapIdxTotal; lapIdx++)
            {
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
                if (root == null)
                {
                    Console.WriteLine($"LapHeader.xml has no root element: {filePath}");
                    return;
                }

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
        /// <param name="runPath">Pfad zum Run-Ordner.</param>
        /// <param name="track">Track-Name.</param>
        /// <param name="trackLen">Track-Länge.</param>
        public void EditSessionXML(string runPath, string track, string trackLen)
        {
            Console.WriteLine($"Writing track \"{track}\" and trackLength \"{trackLen}\" m in Session.xml file.");
            string sessionFilePath = Path.Combine(runPath, "Session.xml");
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
                if (root == null)
                {
                    Console.WriteLine("Session.xml has no root element. Update aborted.");
                    return;
                }

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
        /// <param name="runPath">Pfad zum Run-Ordner.</param>
        /// <param name="alias">Alias für Fahrer/Bike.</param>
        /// <param name="rider">Fahrername.</param>
        public void EditCarXML(string runPath, string alias, string rider)
        {
            Console.WriteLine($"Writing alias \"{alias}\" and rider \"{rider}\" in Car.xml file.");
            string xmlFilePath = Path.Combine(runPath, "Car.xml");
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
                if (root == null)
                {
                    Console.WriteLine($"Car.xml has no root element: {xmlFilePath}");
                    return;
                }

                // Setze Name und DriverName
                bool ok1 = UpdateNode(root, "Name", alias);
                bool ok2 = UpdateNode(root, "DriverName", rider);

                if (!ok1 || !ok2)
                {
                    Console.WriteLine("Missing <Name> or <DriverName> tag in Car.xml. Update aborted.");
                    return;
                }

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
            node.InnerText = val;
            return true;
        }
    }
}