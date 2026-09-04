using KtmDataHandler.Marelli.Wintax;

namespace Ktm.Racing.Core
{
    /// <summary>
    /// Setzt Run- und Lap-Properties über die KtmDataHandler-API (MarelliDataAccess),
    /// anstatt die WinTAX4-XML-Dateien direkt zu manipulieren.
    /// 
    /// Ersetzt die direkte XML-Bearbeitung aus XmlRunEditor durch die offizielle API:
    ///   - AddRunProperty(name, value)   → schreibt Run-Level-Properties (Session.xml / Car.xml)
    ///   - AddLapProperty(nLap, name, value) → schreibt Lap-Level-Properties (LapHeader.xml)
    /// </summary>
    public class RunPropertyEditor
    {
        /// <summary>
        /// Setzt alle Run- und Lap-Properties für einen Run-Ordner.
        /// </summary>
        /// <param name="runPath">Pfad zum Run-Ordner</param>
        /// <param name="alias">Bike/Fahrer-Alias (z.B. "JP01")</param>
        /// <param name="track">Track-Name</param>
        /// <param name="trackLength">Track-Länge in Metern</param>
        /// <param name="rider">Fahrername</param>
        public void SetRunProperties(string runPath, string alias, string track, string trackLength, string rider)
        {
            var ztxAccess = KtmDataHandlerFactory.CreateAccess();
            try
            {
                ztxAccess.SetDataLocation([runPath]);

                // Run-Level Properties (Session.xml / Car.xml Äquivalente)
                SetSessionProperties(ztxAccess, track, trackLength);
                SetCarProperties(ztxAccess, alias, rider);

                // Lap-Level Properties (LapHeader.xml Äquivalente)
                SetLapProperties(ztxAccess, runPath, alias);

                Console.WriteLine($"Run-Properties erfolgreich gesetzt für: {runPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Setzen der Run-Properties: {ex.Message}");
            }
            finally
            {
                ztxAccess.ReleaseObjects();
            }
        }

        /// <summary>
        /// Setzt Session-Level Properties (entspricht Session.xml: Track, TrackLen).
        /// </summary>
        private static void SetSessionProperties(MarelliDataAccess ztxAccess, string track, string trackLength)
        {
            Console.WriteLine($"Setting Track=\"{track}\", TrackLen=\"{trackLength}\" via API.");
            ztxAccess.AddRunProperty("Track", track);
            ztxAccess.AddRunProperty("TrackLen", trackLength);
        }

        /// <summary>
        /// Setzt Car-Level Properties (entspricht Car.xml: Name, DriverName).
        /// </summary>
        private static void SetCarProperties(MarelliDataAccess ztxAccess, string alias, string rider)
        {
            Console.WriteLine($"Setting Name=\"{alias}\", DriverName=\"{rider}\" via API.");
            ztxAccess.AddRunProperty("Name", alias);
            ztxAccess.AddRunProperty("DriverName", rider);
        }

        /// <summary>
        /// Setzt Lap-Level Properties für alle Laps (entspricht LapHeader.xml: Lap, TrackLap, Alias, Run, Marker).
        /// </summary>
        private static void SetLapProperties(MarelliDataAccess ztxAccess, string runPath, string alias)
        {
            // Run-Nummer aus Ordnername extrahieren
            string runFolderName = new DirectoryInfo(runPath).Name;
            int runNumber = int.TryParse(runFolderName.Replace("Run_", ""), out int num) ? num : 0;

            // Anzahl Laps ermitteln über GetLapMarks
            // GetLapMarks gibt die Zeitstempel der Lap-Markierungen zurück
            // Die Anzahl der Laps = LapMarks.Length + 1 (oder mindestens 1)
            var lapMarks = ztxAccess.GetLapMarks();
            int numLaps = (lapMarks != null && lapMarks.Length > 0) ? lapMarks.Length + 1 : 1;

            Console.WriteLine($"Setting Lap-Properties for {numLaps} laps, Alias=\"{alias}\", Run={runNumber}.");

            for (int lap = 1; lap <= numLaps; lap++)
            {
                ztxAccess.AddLapProperty(lap, "Lap", lap);
                ztxAccess.AddLapProperty(lap, "TrackLap", lap);
                ztxAccess.AddLapProperty(lap, "Alias", alias);
                ztxAccess.AddLapProperty(lap, "Run", runNumber);
                ztxAccess.AddLapProperty(lap, "Marker", (lap < numLaps) ? "Lap" : "Box");
            }
        }
    }
}
