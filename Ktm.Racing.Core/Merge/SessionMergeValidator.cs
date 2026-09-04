using KtmDataHandler.Marelli.Wintax;
using KtmDataHandler.Models.Channels;

namespace Ktm.Racing.Core
{
    /// <summary>
    /// Statische Validierungsmethoden für den Session-Merge-Prozess.
    /// 
    /// Prüft alle Eingabeparameter und Zwischenergebnisse, bevor die
    /// KtmDataHandler-API angesprochen wird. Fehler werden frühzeitig erkannt
    /// und mit spezifischen Ausnahmen oder Rückgabewerten kommuniziert.
    /// 
    /// <para><b>Validierungsebenen:</b></para>
    /// <list type="number">
    ///   <item>Pfad-Validierung: Existenz, Verzeichnisstruktur</item>
    ///   <item>Run-Validierung: IsRunLocation, Lap-Ordner vorhanden</item>
    ///   <item>Channel-Validierung: Null-Prüfung, leere Arrays, Datenformat</item>
    ///   <item>Timeseries-Validierung: IsNullOrEmpty, SignalData-Integrität</item>
    /// </list>
    /// </summary>
    public static class SessionMergeValidator
    {
        /// <summary>
        /// Validiert den Eingabepfad für den Session-Merge.
        /// Prüft ob der Pfad existiert und ein gültiges Verzeichnis ist.
        /// </summary>
        /// <param name="path">Zu validierender Pfad (Session- oder Rider-Ordner)</param>
        /// <exception cref="ArgumentNullException">Wenn <paramref name="path"/> null oder leer ist</exception>
        /// <exception cref="DirectoryNotFoundException">Wenn der Pfad nicht existiert</exception>
        public static void ValidatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path),
                    "Der Pfad für den Session-Merge darf nicht null oder leer sein.");
            }

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(
                    $"Das Verzeichnis für den Session-Merge existiert nicht: {path}");
            }
        }

        /// <summary>
        /// Validiert die IMarelliDataBrowse-Instanz.
        /// </summary>
        /// <param name="browse">Zu validierende Browse-Instanz</param>
        /// <exception cref="ArgumentNullException">Wenn <paramref name="browse"/> null ist</exception>
        public static void ValidateBrowseInstance(IMarelliDataBrowse browse)
        {
            if (browse is null)
            {
                throw new ArgumentNullException(nameof(browse),
                    "IMarelliDataBrowse-Instanz darf nicht null sein. " +
                    "Erstellen Sie eine Instanz via: new MarelliDataBrowse()");
            }
        }

        /// <summary>
        /// Validiert die Liste der entdeckten Run-Pfade.
        /// </summary>
        /// <param name="runPaths">Array der gefundenen Run-Pfade</param>
        /// <returns>True wenn mindestens ein Run vorhanden ist, false bei 0 Runs (kein Fehler)</returns>
        public static bool ValidateDiscoveredRuns(string[]? runPaths)
        {
            if (runPaths is null || runPaths.Length == 0)
            {
                Console.WriteLine("Session-Merge: Keine Runs im angegebenen Verzeichnis gefunden.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validiert einen einzelnen Run-Pfad über die IMarelliDataBrowse-API.
        /// 
        /// Prüft:
        /// - Verzeichnis existiert
        /// - IsRunLocation gibt true zurück (Lap_*-Ordner mit cableData.ztx vorhanden)
        /// </summary>
        /// <param name="runPath">Pfad zum Run-Ordner</param>
        /// <param name="browse">IMarelliDataBrowse-Instanz für die Run-Validierung</param>
        /// <returns>
        /// Tuple: (isValid, errorMessage).
        /// isValid=true wenn der Run gültig ist, false mit Fehlerbeschreibung sonst.
        /// </returns>
        public static (bool IsValid, string? ErrorMessage) ValidateRunPath(
            string runPath, IMarelliDataBrowse browse)
        {
            if (string.IsNullOrWhiteSpace(runPath))
            {
                return (false, "Run-Pfad ist null oder leer.");
            }

            if (!Directory.Exists(runPath))
            {
                return (false, $"Run-Verzeichnis existiert nicht: {runPath}");
            }

            // IsRunLocation prüft: Lap_*-Unterordner + cableData.ztx + Run_-Prefix
            if (!browse.IsRunLocation(runPath))
            {
                return (false, $"Kein gültiger WinTAX4-Run (fehlende Lap-Ordner oder cableData.ztx): {runPath}");
            }

            return (true, null);
        }

        /// <summary>
        /// Validiert die nach dem Laden eines Runs zurückgegebenen Channel-Namen.
        /// </summary>
        /// <param name="channelNames">Channel-Namen vom Run</param>
        /// <param name="runPath">Run-Pfad (für Fehlermeldung)</param>
        /// <returns>True wenn Channels vorhanden sind, false bei null oder leerem Array</returns>
        public static bool ValidateChannelNames(string[]? channelNames, string runPath)
        {
            if (channelNames is null || channelNames.Length == 0)
            {
                Console.WriteLine($"Session-Merge: Run hat keine Channels: {Path.GetFileName(runPath)}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validiert eine ITimeseries-Instanz vor dem Concat.
        /// 
        /// Prüft:
        /// - Nicht null
        /// - Nicht leer (via IsNullOrEmpty)
        /// - SignalData vorhanden
        /// </summary>
        /// <param name="timeseries">Zu validierende Timeseries</param>
        /// <param name="channelName">Channel-Name (für Fehlermeldung)</param>
        /// <returns>True wenn die Timeseries gültig und nicht leer ist</returns>
        public static bool ValidateTimeseries(ITimeseries? timeseries, string channelName)
        {
            if (timeseries is null)
            {
                Console.WriteLine($"Session-Merge: Timeseries ist null für Channel '{channelName}' (übersprungen).");
                return false;
            }

            if (timeseries.IsNullOrEmpty())
            {
                Console.WriteLine($"Session-Merge: Timeseries ist leer für Channel '{channelName}' (übersprungen).");
                return false;
            }

            if (timeseries.SignalData is null)
            {
                Console.WriteLine($"Session-Merge: SignalData ist null für Channel '{channelName}' (übersprungen).");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validiert ein IDataset nach dem Laden eines Runs.
        /// </summary>
        /// <param name="dataset">Geladenes Dataset</param>
        /// <param name="runPath">Run-Pfad (für Fehlermeldung)</param>
        /// <returns>True wenn das Dataset gültig und nicht leer ist</returns>
        public static bool ValidateDataset(IDataset? dataset, string runPath)
        {
            if (dataset is null)
            {
                Console.WriteLine($"Session-Merge: Dataset ist null für Run: {Path.GetFileName(runPath)}");
                return false;
            }

            if (dataset.NumElements() == 0)
            {
                Console.WriteLine($"Session-Merge: Dataset enthält keine Elemente für Run: {Path.GetFileName(runPath)}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validiert die Rückgabe von LoadRun().
        /// LoadRun gibt die Anzahl geladener Runs zurück: -1 bei Fehler, ≥0 bei Erfolg.
        /// </summary>
        /// <param name="loadResult">Rückgabewert von LoadRun()</param>
        /// <param name="runPath">Run-Pfad (für Fehlermeldung)</param>
        /// <returns>True wenn der Run erfolgreich geladen wurde</returns>
        public static bool ValidateLoadRunResult(int loadResult, string runPath)
        {
            if (loadResult < 0)
            {
                Console.WriteLine(
                    $"Session-Merge: Fehler beim Laden des Runs (LoadRun={loadResult}): {Path.GetFileName(runPath)}");
                return false;
            }

            return true;
        }
    }
}
