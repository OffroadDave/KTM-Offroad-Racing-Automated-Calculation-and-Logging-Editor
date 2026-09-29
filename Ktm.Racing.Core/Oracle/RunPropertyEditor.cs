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
        /// <param name="lapOffset">Basis für fortlaufende Lap-Nummerierung über mehrere Renn-Runs (Option C).</param>
        /// <param name="isLastInStint">True wenn dies der letzte Renn-Run ist (nur dann Box-Marker am Ende).</param>
        /// <returns>Anzahl der gesetzten Laps in diesem Run (für die Offset-Akkumulation).</returns>
        public int SetRunProperties(string runPath, string alias, string track, string trackLength, string rider,
            int lapOffset = 0, bool isLastInStint = true)
        {
            var ztxAccess = KtmDataHandlerFactory.CreateAccess();
            int numLaps = 0;
            try
            {
                ztxAccess.SetDataLocation([runPath]);

                // Run-Level Properties (Session.xml / Car.xml Äquivalente)
                SetSessionProperties(ztxAccess, track, trackLength);
                SetCarProperties(ztxAccess, alias, rider);

                // Lap-Level Properties (LapHeader.xml Äquivalente); lapOffset ermöglicht
                // fortlaufende Nummerierung über mehrere Renn-Runs hinweg (Option C).
                numLaps = SetLapProperties(ztxAccess, runPath, alias, lapOffset, isLastInStint);

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
            return numLaps;
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
        private static int SetLapProperties(MarelliDataAccess ztxAccess, string runPath, string alias,
            int lapOffset, bool isLastInStint)
        {
            // Run-Nummer aus Ordnername extrahieren
            string runFolderName = new DirectoryInfo(runPath).Name;
            int runNumber = int.TryParse(runFolderName.Replace("Run_", ""), out int num) ? num : 0;

            // Anzahl Laps = Anzahl der Lap_*-Ordner (jede Lap ist eine eigene Akquisition).
            // Deterministisch und ohne LoadRun; GetLapMarks warf hier NRE (Daten nicht geladen).
            int numLaps = Math.Max(1,
                Directory.GetDirectories(runPath, "Lap_*", SearchOption.TopDirectoryOnly).Length);

            Console.WriteLine($"Setting Lap-Properties for {numLaps} laps (offset {lapOffset}), Alias=\"{alias}\", Run={runNumber}.");

            for (int lap = 1; lap <= numLaps; lap++)
            {
                // Lap/TrackLap erhalten die fortlaufende Nummer über den Stint (Offset + lokaler Lap).
                int continuousLap = lapOffset + lap;
                ztxAccess.AddLapProperty(lap, "Lap", continuousLap);
                ztxAccess.AddLapProperty(lap, "TrackLap", continuousLap);
                ztxAccess.AddLapProperty(lap, "Alias", alias);
                ztxAccess.AddLapProperty(lap, "Run", runNumber);

                // Box-Marker nur am tatsächlichen Stint-Ende; Zwischen-Runs enden mit "Lap".
                bool lastLapOfStint = isLastInStint && lap == numLaps;
                ztxAccess.AddLapProperty(lap, "Marker", lastLapOfStint ? "Box" : "Lap");
            }

            return numLaps;
        }
    }
}
