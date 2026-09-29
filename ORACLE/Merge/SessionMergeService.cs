using KtmDataHandler.Marelli.Wintax;
using KtmDataHandler.Models.Channels;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Service für das Zusammenführen aller Runs einer Session zu einem konsolidierten Dataset.
    /// 
    /// <para><b>Verarbeitungsablauf:</b></para>
    /// <list type="number">
    ///   <item>Run-Discovery: Alle gültigen Runs im Rider-Ordner via <see cref="IMarelliDataBrowse.FindRuns"/> finden</item>
    ///   <item>Run-Loading: Jeden Run einzeln laden via <see cref="IMarelliDataAccessRead.SetDataLocation"/> + <see cref="IMarelliDataAccessRead.LoadRun()"/></item>
    ///   <item>Channel-Extraktion: Alle Channels pro Run via <see cref="IGettableChannels.GetChannels(string[], double, double, string[], bool)"/> abrufen</item>
    ///   <item>Channel-Merge: Channels über Runs via <see cref="ITimeseries.Concat"/> verketten</item>
    ///   <item>Ergebnis: <see cref="MergedSessionData"/> mit konsolidiertem <see cref="IDataset"/></item>
    /// </list>
    /// 
    /// <para><b>Verwendete KtmDataHandler-APIs:</b></para>
    /// <list type="bullet">
    ///   <item><see cref="IMarelliDataBrowse.FindRuns"/> — Run-Discovery in einem Verzeichnisbaum</item>
    ///   <item><see cref="IMarelliDataBrowse.IsRunLocation"/> — Validierung einzelner Run-Pfade</item>
    ///   <item><see cref="IMarelliDataBrowse.GetSessionName"/> — Session-Name aus Ordnerstruktur</item>
    ///   <item><see cref="IMarelliDataBrowse.GetDeviceName"/> — Rider/Device-Name aus Ordnerstruktur</item>
    ///   <item><see cref="IMarelliDataAccessRead.LoadRun()"/> — Run-Daten laden (alle Laps automatisch)</item>
    ///   <item><see cref="IGettableChannels.GetChannelsNames"/> — Verfügbare Channel-Namen abfragen</item>
    ///   <item><see cref="IGettableChannels.GetChannels(string[], double, double, string[], bool)"/> — Channel-Daten als IDataset laden</item>
    ///   <item><see cref="IMarelliDataAccessRead.GetLastSampleTime"/> — Letzte Sample-Zeit für Zeitoffset</item>
    ///   <item><see cref="ITimeseries.Concat"/> — Zwei Zeitreihen verketten (Kern-Merge-Operation)</item>
    ///   <item><see cref="ITimeseries.Clone"/> — Defensive Kopie der ersten Timeseries vor Concat</item>
    /// </list>
    /// 
    /// <para><b>Performance-Hinweise:</b></para>
    /// <list type="bullet">
    ///   <item>Pro Run wird ein eigener <see cref="IMarelliDataAccessRead"/> erzeugt und nach Benutzung freigegeben</item>
    ///   <item>Channel-Union wird einmalig vor dem Merge berechnet, kein mehrfaches Collection-Durchlaufen</item>
    ///   <item>Concat modifiziert die Ziel-Timeseries in-place → nur erste Timeseries wird geklont</item>
    /// </list>
    /// 
    /// <para><b>Erweiterungspunkte:</b></para>
    /// <list type="bullet">
    ///   <item>Channel-Filter: Nur bestimmte Channels mergen (channelFilter-Parameter)</item>
    ///   <item>Resampling: Unterschiedliche Abtastraten über <see cref="ITimeseries.Resample"/> angleichen</item>
    ///   <item>Multi-Rider: Session-Pfad statt Rider-Pfad übergeben → mehrere MergedSessionData</item>
    ///   <item>Export: Merge-Ergebnis als neuen WinTAX4-Run via <see cref="IMarelliDataAccessWrite"/> speichern</item>
    /// </list>
    /// </summary>
    public class SessionMergeService
    {
        private readonly IMarelliDataBrowse _browse;

        /// <summary>
        /// Erstellt eine neue Instanz des SessionMergeService.
        /// </summary>
        /// <param name="browse">
        /// IMarelliDataBrowse-Instanz für Run-Discovery und Pfad-Parsing.
        /// Typisch: <c>new MarelliDataBrowse()</c>
        /// </param>
        /// <exception cref="ArgumentNullException">Wenn <paramref name="browse"/> null ist</exception>
        public SessionMergeService(IMarelliDataBrowse browse)
        {
            SessionMergeValidator.ValidateBrowseInstance(browse);
            _browse = browse;
        }

        /// <summary>
        /// Führt alle Runs eines Riders innerhalb einer Session zusammen.
        /// 
        /// <para><b>Eingabe:</b> Pfad zum Rider-Ordner (z.B. <c>...\R1\AA01</c>)
        /// oder zum Session-Ordner (z.B. <c>...\R1</c>).
        /// <see cref="IMarelliDataBrowse.FindRuns"/> durchsucht rekursiv nach cableData.ztx.</para>
        /// 
        /// <para><b>Ablauf pro Run:</b></para>
        /// <list type="number">
        ///   <item>Validierung via <see cref="SessionMergeValidator.ValidateRunPath"/></item>
        ///   <item>Reader erzeugen via <see cref="KtmDataHandlerFactory.CreateReader"/></item>
        ///   <item><c>SetDataLocation</c> + <c>LoadRun()</c></item>
        ///   <item>Channel-Namen + Daten laden</item>
        ///   <item>Reader freigeben via <c>ReleaseObjects()</c></item>
        /// </list>
        /// 
        /// <para><b>Merge-Strategie:</b> Für jeden Channel wird beim ersten Auftreten
        /// ein Clone erstellt. Alle weiteren Runs werden via <see cref="ITimeseries.Concat"/>
        /// angehängt. Das Ergebnis-Dataset enthält die konkatenierten Timeseries.</para>
        /// </summary>
        /// <param name="riderPath">
        /// Pfad zum Rider-Ordner (z.B. <c>...\R1\AA01</c>) oder Session-Ordner.
        /// FindRuns durchsucht rekursiv nach gültigen Runs.
        /// </param>
        /// <returns>
        /// <see cref="MergedSessionData"/> mit dem konsolidierten Dataset.
        /// Bei 0 gültigen Runs: <see cref="MergedSessionData.HasData"/> = false.
        /// </returns>
        /// <exception cref="ArgumentNullException">Wenn <paramref name="riderPath"/> null/leer ist</exception>
        /// <exception cref="DirectoryNotFoundException">Wenn <paramref name="riderPath"/> nicht existiert</exception>
        public MergedSessionData MergeSession(string riderPath)
        {
            // --- 1. Eingabe-Validierung ---
            SessionMergeValidator.ValidatePath(riderPath);

            Console.WriteLine($"Session-Merge: Starte Zusammenführung für: {riderPath}");

            // --- 2. Run-Discovery über IMarelliDataBrowse ---
            var discoveredRunPaths = _browse.FindRuns(riderPath);

            if (!SessionMergeValidator.ValidateDiscoveredRuns(discoveredRunPaths))
            {
                return BuildEmptyResult(riderPath);
            }

            // Sortierung nach Ordnernamen für konsistente Reihenfolge (Run_201, Run_202, ...)
            var sortedRunPaths = discoveredRunPaths
                .OrderBy(p => Path.GetFileName(p), StringComparer.OrdinalIgnoreCase)
                .ToArray();

            Console.WriteLine($"Session-Merge: {sortedRunPaths.Length} Run(s) entdeckt.");

            // --- 3. Session-/Rider-Metadaten aus erstem gültigen Run extrahieren ---
            var (sessionName, riderName) = ExtractSessionMetadata(sortedRunPaths);

            // --- 4. Alle Runs laden und Channel-Daten sammeln ---
            //
            // runInfos:      Detailinfo pro Run (inkl. Status)
            // runDatasets:    Geladene Datasets pro erfolgreichem Run (Index-korreliert mit runInfos)
            // allChannelNames: Union aller Channel-Namen über alle Runs (einmalig berechnet)
            var runInfos = new List<MergedRunInfo>(sortedRunPaths.Length);
            var runDatasets = new List<(string RunPath, IDataset Dataset)>();
            var allChannelNames = new HashSet<string>(StringComparer.Ordinal);

            foreach (var runPath in sortedRunPaths)
            {
                var runInfo = LoadSingleRun(runPath, allChannelNames);
                runInfos.Add(runInfo.Info);

                if (runInfo.Info.Status == RunMergeStatus.Success && runInfo.Dataset is not null)
                {
                    runDatasets.Add((runPath, runInfo.Dataset));
                }
            }

            int mergedCount = runDatasets.Count;
            var channelNameList = allChannelNames.ToList();

            Console.WriteLine(
                $"Session-Merge: {mergedCount} von {sortedRunPaths.Length} Run(s) erfolgreich geladen. " +
                $"{channelNameList.Count} Channel(s) insgesamt.");

            // --- 5. Channel-Merge: Timeseries über alle Runs verketten ---
            //
            // Strategie:
            //   - mergedChannels Dictionary: Channel-Name → akkumulierte ITimeseries
            //   - Erster Run: Clone der Timeseries (defensive Kopie, da Concat in-place modifiziert)
            //   - Folgende Runs: Concat an die bestehende Timeseries
            //   - Channels die nur in einigen Runs vorkommen: werden nur aus vorhandenen Runs genommen
            var mergedDataset = MergeChannelsAcrossRuns(runDatasets, channelNameList);

            Console.WriteLine(
                $"Session-Merge: Zusammenführung abgeschlossen. " +
                $"Ergebnis-Dataset enthält {mergedDataset.NumElements()} Channel(s).");

            // --- 6. Ergebnis zusammenbauen ---
            return new MergedSessionData
            {
                SessionName = sessionName,
                RiderName = riderName,
                MergedDataset = mergedDataset,
                Runs = runInfos.AsReadOnly(),
                ChannelNames = channelNameList.AsReadOnly(),
                TotalRunsDiscovered = sortedRunPaths.Length,
                MergedRunCount = mergedCount
            };
        }

        // =====================================================================
        //  Private Hilfsmethoden
        // =====================================================================

        /// <summary>
        /// Extrahiert Session- und Rider-Name aus dem ersten gültigen Run
        /// via <see cref="IMarelliDataBrowse.GetSessionName"/> und <see cref="IMarelliDataBrowse.GetDeviceName"/>.
        /// 
        /// Beide Methoden erfordern einen gültigen Run-Pfad (IsRunLocation = true).
        /// Falls kein Run gültig ist, werden die Ordnernamen als Fallback verwendet.
        /// </summary>
        private (string SessionName, string RiderName) ExtractSessionMetadata(string[] runPaths)
        {
            // Versuche über die API (benötigt einen gültigen Run-Pfad)
            foreach (var runPath in runPaths)
            {
                if (_browse.IsRunLocation(runPath))
                {
                    var session = _browse.GetSessionName(runPath);
                    var device = _browse.GetDeviceName(runPath);

                    if (!string.IsNullOrEmpty(session) && !string.IsNullOrEmpty(device))
                    {
                        return (session, device);
                    }
                }
            }

            // Fallback: Ordnernamen aus dem Pfad des ersten Runs
            // WinTAX4-Hierarchie: .../Event/Session/Device/Run_xxx
            // → Device = Parent, Session = Grandparent
            var firstRun = runPaths.FirstOrDefault() ?? "";
            var deviceFolder = Path.GetDirectoryName(firstRun) ?? "";
            var sessionFolder = Path.GetDirectoryName(deviceFolder) ?? "";

            return (
                Path.GetFileName(sessionFolder),
                Path.GetFileName(deviceFolder)
            );
        }

        /// <summary>
        /// Lädt einen einzelnen Run über die KtmDataHandler-API und gibt die Daten zurück.
        /// 
        /// <para><b>Ablauf:</b></para>
        /// <list type="number">
        ///   <item>Run-Pfad validieren via <see cref="SessionMergeValidator.ValidateRunPath"/></item>
        ///   <item>Neuen Reader via <see cref="KtmDataHandlerFactory.CreateReader"/> erzeugen</item>
        ///   <item><c>SetDataLocation</c> → <c>LoadRun()</c></item>
        ///   <item>Channel-Namen abfragen, in allChannelNames einfügen</item>
        ///   <item>Alle Channels laden via <c>GetChannels</c></item>
        ///   <item>Reader freigeben via <c>ReleaseObjects()</c></item>
        /// </list>
        /// 
        /// <para><b>Fehlerbehandlung:</b> Exceptions werden gefangen und als
        /// <see cref="RunMergeStatus.LoadError"/> mit Fehlermeldung zurückgegeben.
        /// Ein fehlerhafter Run stoppt nicht den gesamten Merge-Prozess.</para>
        /// </summary>
        /// <param name="runPath">Pfad zum Run-Ordner</param>
        /// <param name="allChannelNames">
        /// Wird in-place erweitert: Channel-Namen dieses Runs werden hinzugefügt.
        /// Dient als Running-Union über alle Runs.
        /// </param>
        /// <returns>Tuple mit MergedRunInfo und dem geladenen Dataset (null bei Fehler)</returns>
        private (MergedRunInfo Info, IDataset? Dataset) LoadSingleRun(
            string runPath, HashSet<string> allChannelNames)
        {
            string runFolderName = Path.GetFileName(runPath);

            // --- Validierung ---
            var (isValid, errorMessage) = SessionMergeValidator.ValidateRunPath(runPath, _browse);
            if (!isValid)
            {
                // IsRunLocation = false → Run hat keine Lap-Ordner mit cableData.ztx
                // Das ist der erwartete Fall für leere Runs (z.B. Run_201, Run_206)
                var status = Directory.Exists(runPath) ? RunMergeStatus.NoLaps : RunMergeStatus.InvalidPath;
                Console.WriteLine($"Session-Merge: {runFolderName} → {status} ({errorMessage})");

                return (new MergedRunInfo
                {
                    RunPath = runPath,
                    RunFolderName = runFolderName,
                    Status = status,
                    ErrorMessage = errorMessage
                }, null);
            }

            // --- Run laden ---
            IMarelliDataAccessRead reader = KtmDataHandlerFactory.CreateReader();
            try
            {
                reader.SetDataLocation([runPath]);
                int loadResult = reader.LoadRun();

                if (!SessionMergeValidator.ValidateLoadRunResult(loadResult, runPath))
                {
                    return (new MergedRunInfo
                    {
                        RunPath = runPath,
                        RunFolderName = runFolderName,
                        Status = RunMergeStatus.LoadError,
                        ErrorMessage = $"LoadRun fehlgeschlagen (Rückgabewert: {loadResult})"
                    }, null);
                }

                // --- Channel-Namen abfragen ---
                var channelNames = reader.GetChannelsNames();

                if (!SessionMergeValidator.ValidateChannelNames(channelNames, runPath))
                {
                    return (new MergedRunInfo
                    {
                        RunPath = runPath,
                        RunFolderName = runFolderName,
                        Status = RunMergeStatus.NoChannels,
                        ErrorMessage = "Keine Channels im Run vorhanden"
                    }, null);
                }

                // Channel-Namen in die globale Union einfügen (HashSet dedupliziert automatisch)
                foreach (var name in channelNames)
                {
                    allChannelNames.Add(name);
                }

                // --- Alle Channels laden ---
                IDataset dataset = reader.GetChannels(channelNames);

                if (!SessionMergeValidator.ValidateDataset(dataset, runPath))
                {
                    return (new MergedRunInfo
                    {
                        RunPath = runPath,
                        RunFolderName = runFolderName,
                        ChannelNames = channelNames,
                        Status = RunMergeStatus.NoChannels,
                        ErrorMessage = "Dataset leer nach GetChannels"
                    }, null);
                }

                // --- Metadaten sammeln ---
                double lastSampleTime = reader.GetLastSampleTime();

                // Lap-Count über Lap_*-Ordner zählen (analog zu IsRunLocation-Logik)
                int lapCount = Directory.Exists(runPath)
                    ? Directory.GetDirectories(runPath, "Lap_*", SearchOption.TopDirectoryOnly).Length
                    : 0;

                Console.WriteLine(
                    $"Session-Merge: {runFolderName} → OK " +
                    $"({lapCount} Lap(s), {channelNames.Length} Channel(s), LastT={lastSampleTime:F2}s)");

                return (new MergedRunInfo
                {
                    RunPath = runPath,
                    RunFolderName = runFolderName,
                    LapCount = lapCount,
                    ChannelNames = channelNames,
                    LastSampleTime = lastSampleTime,
                    Status = RunMergeStatus.Success
                }, dataset);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Session-Merge: {runFolderName} → Fehler: {ex.Message}");

                return (new MergedRunInfo
                {
                    RunPath = runPath,
                    RunFolderName = runFolderName,
                    Status = RunMergeStatus.LoadError,
                    ErrorMessage = ex.Message
                }, null);
            }
            finally
            {
                // Reader-Ressourcen immer freigeben (analog zu ImportDataCalculator-Pattern)
                reader.ReleaseObjects();
            }
        }

        /// <summary>
        /// Verkettet die Channel-Daten aus allen erfolgreich geladenen Runs zu einem Dataset.
        /// 
        /// <para><b>Merge-Algorithmus:</b></para>
        /// <list type="number">
        ///   <item>Für jeden Channel-Namen aus der Union aller Runs:</item>
        ///   <item>Iteriere über alle Run-Datasets in chronologischer Reihenfolge</item>
        ///   <item>Erster Run mit diesem Channel: <c>Clone()</c> der Timeseries als Basis</item>
        ///   <item>Folgende Runs: <c>Concat()</c> hängt die Daten an die Basis an</item>
        ///   <item>Ergebnis in das Merge-Dataset via <c>Add()</c> einfügen</item>
        /// </list>
        /// 
        /// <para><b>Hinweis zu Concat():</b>
        /// <c>ITimeseries.Concat</c> ist eine void-Methode die in-place modifiziert.
        /// Laut API-Doku: "The method supposes that the parameter object 'second' must be
        /// added at the end of the current object. No Time coherence check is performed."
        /// Daher wird die erste Timeseries geklont, um das Quell-Dataset nicht zu verändern.</para>
        /// 
        /// <para><b>Edge-Case: Channel fehlt in einem Run.</b>
        /// Wenn ein Channel nur in einigen Runs vorkommt, wird er nur aus den vorhandenen
        /// Runs verkettet. Es entstehen keine Lücken, aber die Zeitachse hat Sprünge.</para>
        /// 
        /// <para><b>Komplexität:</b> O(C × R) wobei C = Anzahl Channels, R = Anzahl Runs.
        /// Die tatsächliche Concat-Operation ist O(n) pro Timeseries-Länge.</para>
        /// </summary>
        /// <param name="runDatasets">Erfolgreich geladene Datasets, in Run-Reihenfolge</param>
        /// <param name="channelNames">Union aller Channel-Namen</param>
        /// <returns>Konsolidiertes Dataset mit allen verketteten Channels</returns>
        private static IDataset MergeChannelsAcrossRuns(
            List<(string RunPath, IDataset Dataset)> runDatasets,
            List<string> channelNames)
        {
            var mergedDataset = new Dataset();

            if (runDatasets.Count == 0 || channelNames.Count == 0)
            {
                return mergedDataset;
            }

            // Sonderfall: Nur ein Run → direkt alle Channels übernehmen (kein Concat nötig)
            if (runDatasets.Count == 1)
            {
                var singleDataset = runDatasets[0].Dataset;

                foreach (var channelName in channelNames)
                {
                    var timeseries = singleDataset.GetElement(channelName);
                    if (SessionMergeValidator.ValidateTimeseries(timeseries, channelName))
                    {
                        // Clone um Unabhängigkeit vom Quell-Reader zu gewährleisten
                        mergedDataset.Add((ITimeseries)timeseries.Clone());
                    }
                }

                return mergedDataset;
            }

            // Mehrere Runs: Pro Channel über alle Datasets iterieren und verketten
            // Dictionary für die akkumulierten Timeseries (Wiederverwendung, kein Neuaufbau)
            var mergedChannels = new Dictionary<string, ITimeseries>(
                channelNames.Count, StringComparer.Ordinal);

            foreach (var (runPath, dataset) in runDatasets)
            {
                string runName = Path.GetFileName(runPath);

                foreach (var channelName in channelNames)
                {
                    ITimeseries timeseries = dataset.GetElement(channelName);

                    // Channel existiert nicht in diesem Run → überspringen
                    if (!SessionMergeValidator.ValidateTimeseries(timeseries, channelName))
                    {
                        continue;
                    }

                    if (!mergedChannels.TryGetValue(channelName, out var accumulated))
                    {
                        // Erster Run mit diesem Channel → Clone als Basis
                        // Clone ist nötig, weil Concat in-place modifiziert und
                        // wir das originale Dataset des Readers nicht verändern wollen
                        mergedChannels[channelName] = (ITimeseries)timeseries.Clone();
                    }
                    else
                    {
                        // Folgende Runs → Concat hängt die neuen Daten an
                        // Laut API: "No Time coherence check is performed"
                        // Die Runs sind bereits chronologisch sortiert
                        accumulated.Concat(timeseries);
                    }
                }
            }

            // Alle gemergten Channels in das Ergebnis-Dataset einfügen
            foreach (var kvp in mergedChannels)
            {
                mergedDataset.Add(kvp.Value);
            }

            return mergedDataset;
        }

        /// <summary>
        /// Erzeugt ein leeres MergedSessionData-Ergebnis für den Fall, dass keine Runs gefunden wurden.
        /// </summary>
        private MergedSessionData BuildEmptyResult(string riderPath)
        {
            // Versuche Metadaten aus dem Pfad zu extrahieren (ohne gültigen Run)
            var deviceFolder = Path.GetFileName(riderPath);
            var sessionFolder = Path.GetFileName(Path.GetDirectoryName(riderPath) ?? "");

            return new MergedSessionData
            {
                SessionName = sessionFolder,
                RiderName = deviceFolder,
                TotalRunsDiscovered = 0,
                MergedRunCount = 0
            };
        }
    }
}
