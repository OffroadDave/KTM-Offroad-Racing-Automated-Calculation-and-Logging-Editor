using KtmDataHandler.Marelli.Wintax;
using KtmDataHandler.Models.Channels;

namespace Ktm.Racing.Core
{
    /// <summary>
    /// Zentrale Orchestrierungsklasse für ORACLE-Verarbeitungsschritte.
    /// 
    /// <para><b>Commands:</b></para>
    /// <list type="bullet">
    ///   <item><see cref="ProcessRunAsync"/> — Komplette Postprocessing-Pipeline für einen Run</item>
    ///   <item><see cref="MergeSession"/> — Session-Merge über alle Runs eines Riders</item>
    ///   <item><see cref="ValidateRun"/> — Datenintegritätsprüfung eines Runs</item>
    /// </list>
    /// 
    /// <para>Wird von <c>Program.Main()</c> über Command-Routing aufgerufen.
    /// Jede Methode ist eigenständig nutzbar (für KRAKEN als separate EXE-Jobs
    /// oder für zukünftige Library-Nutzung).</para>
    /// 
    /// <para><b>Abhängigkeiten:</b> Via Constructor Injection für Testbarkeit.
    /// Verwende <see cref="Create()"/> für Standard-Instanziierung oder den Konstruktor
    /// für Unit-Tests mit gemockten Abhängigkeiten.</para>
    /// </summary>
    public class OraclePipeline
    {
        private readonly IMarelliDataBrowse _browse;
        private readonly OracleConfig _config;

        /// <summary>
        /// Erstellt eine neue OraclePipeline mit expliziten Abhängigkeiten.
        /// Primär für Unit-Tests mit gemockten Services.
        /// </summary>
        /// <param name="browse">IMarelliDataBrowse für Run-Discovery und Pfad-Validierung</param>
        /// <param name="config">ORACLE-Konfiguration (Pfade, TeamMappings)</param>
        public OraclePipeline(IMarelliDataBrowse browse, OracleConfig config)
        {
            ArgumentNullException.ThrowIfNull(browse);
            ArgumentNullException.ThrowIfNull(config);
            _browse = browse;
            _config = config;
        }

        /// <summary>
        /// Factory-Methode: Erstellt eine OraclePipeline mit Standard-Abhängigkeiten.
        /// Lädt die Konfiguration aus <c>oracle-config.json</c> und erzeugt eine
        /// neue <see cref="MarelliDataBrowse"/>-Instanz.
        /// </summary>
        public static OraclePipeline Create()
        {
            return new OraclePipeline(new MarelliDataBrowse(), OracleConfig.Load());
        }
        /// <summary>
        /// Komplette Postprocessing-Pipeline für einen einzelnen Run:
        /// Metadaten → Properties → Berechnungen.
        /// 
        /// <para><b>Schritte:</b></para>
        /// <list type="number">
        ///   <item>Idempotenz-Prüfung (oracle.status.json)</item>
        ///   <item>Run-Ordner validieren (IsRunLocation)</item>
        ///   <item>Konfiguration laden (oracle-config.json)</item>
        ///   <item>Metadaten ermitteln (HED-Datei oder KOMPASS-Fallback)</item>
        ///   <item>Gültige Runs discovern</item>
        ///   <item>Run/Lap-Properties setzen</item>
        ///   <item>Berechnungspipeline ausführen (sVehicle etc.)</item>
        ///   <item>Status-Datei schreiben</item>
        /// </list>
        /// </summary>
        /// <param name="runPath">Pfad zum Run_xxx-Ordner</param>
        /// <param name="force">True um Idempotenz-Prüfung zu überspringen</param>
        /// <returns>Exit-Code für KRAKEN</returns>
        public async Task<OracleExitCode> ProcessRunAsync(string runPath, bool force = false)
        {
            Console.WriteLine($"[process] Run Path: {runPath}");

            if (!Directory.Exists(runPath))
            {
                Console.WriteLine($"Der angegebene Run Path existiert nicht: {runPath}");
                return OracleExitCode.DataNotFound;
            }

            // --- Idempotenz-Prüfung ---
            if (!force && OracleStatus.IsAlreadyProcessed(runPath))
            {
                Console.WriteLine("[process] Run bereits erfolgreich verarbeitet (oracle.status.json). Überspringe.");
                Console.WriteLine("[process] Verwende --force um erneut zu verarbeiten.");
                return OracleExitCode.Success;
            }

            var status = OracleStatus.Create(runPath);

            if (!_browse.IsRunLocation(runPath))
            {
                Console.WriteLine($"Warnung: Der Pfad ist kein gültiger WinTAX4-Run: {runPath}");
            }

            // --- Metadaten ermitteln ---
            var metadata = await ResolveMetadataAsync(runPath);
            if (metadata == null)
            {
                Console.WriteLine("Metadaten konnten nicht ermittelt werden.");
                status.SetStep("metadata", StepStatus.Failed);
                status.ExitCode = (int)OracleExitCode.DataNotFound;
                status.Save(runPath);
                return OracleExitCode.DataNotFound;
            }
            status.SetStep("metadata", StepStatus.Success, new Dictionary<string, object>
            {
                ["bikeName"] = metadata.BikeName,
                ["riderName"] = metadata.RiderName,
            });

            // --- Gültige Runs finden ---
            var validRuns = DiscoverRuns(runPath);
            if (validRuns.Count == 0)
            {
                Console.WriteLine($"Keine gültigen Runs gefunden in: {runPath}");
                status.SetStep("discovery", StepStatus.Failed);
                status.ExitCode = (int)OracleExitCode.DataNotFound;
                status.Save(runPath);
                return OracleExitCode.DataNotFound;
            }

            Console.WriteLine($"{validRuns.Count} gültige(r) Run(s) gefunden.");

            // --- Run/Lap-Properties setzen ---
            var propertyEditor = new RunPropertyEditor();
            foreach (var run in validRuns)
            {
                propertyEditor.SetRunProperties(run, metadata.BikeName, metadata.TrackName,
                    metadata.TrackLength, metadata.RiderName);
            }
            status.SetStep("properties", StepStatus.Success);

            // --- Berechnungspipeline für alle Runs ausführen ---
            var importCalculator = new ImportDataCalculator();
            int calcSuccess = importCalculator.RunCalculationPipelineForRuns(validRuns);
            var calcStatus = calcSuccess == validRuns.Count ? StepStatus.Success
                           : calcSuccess > 0 ? StepStatus.Partial
                           : StepStatus.Failed;
            status.SetStep("calculations", calcStatus, new Dictionary<string, object>
            {
                ["processedRuns"] = calcSuccess,
                ["totalRuns"] = validRuns.Count,
            });

            // --- Status-Datei schreiben ---
            status.ExitCode = (int)OracleExitCode.Success;
            status.Save(runPath);

            Console.WriteLine("[process] Abgeschlossen.");
            return OracleExitCode.Success;
        }

        /// <summary>
        /// Session-Merge: Alle Runs eines Riders zusammenführen und als WinTAX4-Run exportieren.
        /// 
        /// <para><b>Schritte:</b></para>
        /// <list type="number">
        ///   <item>Rider-Ordner validieren</item>
        ///   <item>Alle Runs im Ordner entdecken</item>
        ///   <item>Channels über Runs zusammenführen (Concat)</item>
        ///   <item>MergedSessionData erzeugen</item>
        ///   <item>Ergebnis als neuen WinTAX4-Run exportieren via <see cref="SessionMergeExporter"/></item>
        /// </list>
        /// </summary>
        /// <param name="riderPath">Pfad zum Rider-/Device-Ordner (enthält Run_*-Unterordner)</param>
        /// <param name="force">True um Idempotenz-Prüfung zu überspringen</param>
        /// <returns>Exit-Code für KRAKEN</returns>
        public OracleExitCode MergeSession(string riderPath, bool force = false)
        {
            Console.WriteLine($"[merge] Rider Path: {riderPath}");

            if (!Directory.Exists(riderPath))
            {
                Console.WriteLine($"Der angegebene Rider Path existiert nicht: {riderPath}");
                return OracleExitCode.DataNotFound;
            }

            // --- Idempotenz-Prüfung (merge-spezifische Status-Datei) ---
            var mergeStatusPath = Path.Combine(riderPath, ".oracle-merge");
            if (!Directory.Exists(mergeStatusPath))
                Directory.CreateDirectory(mergeStatusPath);

            if (!force && OracleStatus.IsAlreadyProcessed(mergeStatusPath))
            {
                Console.WriteLine("[merge] Session bereits erfolgreich gemergt (oracle.status.json). Überspringe.");
                Console.WriteLine("[merge] Verwende --force um erneut zu mergen.");
                return OracleExitCode.Success;
            }

            var status = OracleStatus.Create(riderPath);

            var mergeService = new SessionMergeService(_browse);
            var mergedSession = mergeService.MergeSession(riderPath);

            if (mergedSession.HasData)
            {
                Console.WriteLine($"[merge] Session-Merge abgeschlossen: " +
                    $"Session=\"{mergedSession.SessionName}\", Rider=\"{mergedSession.RiderName}\", " +
                    $"{mergedSession.MergedRunCount}/{mergedSession.TotalRunsDiscovered} Run(s) zusammengeführt, " +
                    $"{mergedSession.ChannelNames.Count} Channel(s).");

                status.SetStep("merge", StepStatus.Success, new Dictionary<string, object>
                {
                    ["mergedRuns"] = mergedSession.MergedRunCount,
                    ["totalRuns"] = mergedSession.TotalRunsDiscovered,
                    ["channels"] = mergedSession.ChannelNames.Count,
                });

                // --- Export: Merge-Ergebnis als neuen WinTAX4-Run persistieren ---
                var exporter = new SessionMergeExporter();
                var exportedPath = exporter.Export(mergedSession, riderPath);

                if (exportedPath is not null)
                {
                    status.SetStep("export", StepStatus.Success, new Dictionary<string, object>
                    {
                        ["outputPath"] = exportedPath,
                    });
                }
                else
                {
                    status.SetStep("export", StepStatus.Failed);
                    Console.WriteLine("[merge] ⚠️ Export fehlgeschlagen, Merge-Daten nur im Speicher vorhanden.");
                }

                status.ExitCode = (int)OracleExitCode.Success;
                status.Save(mergeStatusPath);
                return OracleExitCode.Success;
            }

            Console.WriteLine("[merge] Keine Daten zum Zusammenführen gefunden.");
            status.SetStep("merge", StepStatus.Failed);
            status.ExitCode = (int)OracleExitCode.DataNotFound;
            status.Save(mergeStatusPath);
            return OracleExitCode.DataNotFound;
        }

        /// <summary>
        /// Verarbeitet ALLE Runs unterhalb eines Track-/Session-/Rider-Ordners: process je Run + merge je Rider.
        /// Respektiert die oracle.status.json-Idempotenz (bereits verarbeitete Runs werden übersprungen, außer <paramref name="force"/>).
        /// </summary>
        /// <param name="folderPath">Track-, Session- oder Rider-Ordner</param>
        /// <param name="force">True überspringt die Idempotenz-Prüfung</param>
        /// <param name="progress">Optionaler Fortschritt für die GUI</param>
        public async Task<OracleExitCode> ProcessFolderAsync(
            string folderPath, bool force = false, IProgress<string>? progress = null)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"[process-folder] Ordner existiert nicht: {folderPath}");
                return OracleExitCode.DataNotFound;
            }

            var runPaths = (_browse.FindRuns(folderPath) ?? Enumerable.Empty<string>())
                .OrderBy(p => Path.GetFileName(p), StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (runPaths.Count == 0)
            {
                progress?.Report("Keine Runs im Ordner gefunden.");
                return OracleExitCode.DataNotFound;
            }

            int ok = 0, failed = 0;
            foreach (var runPath in runPaths)
            {
                progress?.Report($"ORACLE process: {Path.GetFileName(runPath)}");
                var code = await ProcessRunAsync(runPath, force);
                if (code == OracleExitCode.Success) ok++; else failed++;
            }

            // Merge temporär deaktiviert – wird separat überarbeitet.
            // foreach (var riderPath in runPaths.Select(p => Path.GetDirectoryName(p)!).Distinct(StringComparer.OrdinalIgnoreCase))
            // {
            //     progress?.Report($"ORACLE merge: {Path.GetFileName(riderPath)}");
            //     MergeSession(riderPath, force);
            // }

            progress?.Report($"ORACLE fertig: {ok} OK, {failed} Fehler von {runPaths.Count} Run(s).");
            if (failed == 0) return OracleExitCode.Success;
            return ok > 0 ? OracleExitCode.PartialSuccess : OracleExitCode.GeneralError;
        }

        /// <summary>
        /// Validiert die Datenintegrität eines Runs:
        /// Prüft Dateistruktur, Ladbarkeit und vorhandene Channels.
        /// 
        /// <para><b>Prüfungen:</b></para>
        /// <list type="bullet">
        ///   <item>Run-Ordner existiert und ist gültiger WinTAX4-Run</item>
        ///   <item>Mindestens ein Lap_*-Ordner mit cableData.ztx vorhanden</item>
        ///   <item>cableData.ztx Dateien haben plausible Größe (> 0 Bytes)</item>
        ///   <item>Daten können über KtmDataHandler geladen werden</item>
        ///   <item>Mindestens ein Channel ist vorhanden</item>
        /// </list>
        /// </summary>
        /// <param name="runPath">Pfad zum Run_xxx-Ordner</param>
        /// <returns>Exit-Code: Success=OK, DataNotFound=Fehler</returns>
        public OracleExitCode ValidateRun(string runPath)
        {
            Console.WriteLine($"[validate] Run Path: {runPath}");

            // --- Existenzprüfung ---
            if (!Directory.Exists(runPath))
            {
                Console.WriteLine($"Run-Ordner existiert nicht: {runPath}");
                return OracleExitCode.DataNotFound;
            }

            // --- WinTAX4-Run-Validierung ---
            if (!_browse.IsRunLocation(runPath))
            {
                Console.WriteLine($"Kein gültiger WinTAX4-Run: {runPath}");
                return OracleExitCode.DataNotFound;
            }

            // --- Lap-Ordner prüfen ---
            var lapDirs = Directory.GetDirectories(runPath, "Lap_*", SearchOption.TopDirectoryOnly);
            if (lapDirs.Length == 0)
            {
                Console.WriteLine("Keine Lap_*-Ordner gefunden.");
                return OracleExitCode.DataNotFound;
            }
            Console.WriteLine($"  {lapDirs.Length} Lap-Ordner gefunden.");

            // --- cableData.ztx Dateien prüfen ---
            int validLaps = 0;
            int emptyFiles = 0;
            foreach (var lapDir in lapDirs)
            {
                var ztxFile = Path.Combine(lapDir, "cableData.ztx");
                if (!File.Exists(ztxFile))
                {
                    Console.WriteLine($"  ⚠️ cableData.ztx fehlt in: {Path.GetFileName(lapDir)}");
                    continue;
                }

                var fileInfo = new FileInfo(ztxFile);
                if (fileInfo.Length == 0)
                {
                    Console.WriteLine($"  ⚠️ cableData.ztx ist leer (0 Bytes): {Path.GetFileName(lapDir)}");
                    emptyFiles++;
                    continue;
                }

                validLaps++;
            }

            if (validLaps == 0)
            {
                Console.WriteLine("Keine gültigen cableData.ztx Dateien gefunden.");
                return OracleExitCode.DataNotFound;
            }

            Console.WriteLine($"  {validLaps} gültige cableData.ztx Datei(en), {emptyFiles} leer.");

            // --- Daten ladbar prüfen ---
            var ztxRead = KtmDataHandlerFactory.CreateReader();
            try
            {
                ztxRead.SetDataLocation([runPath]);
                ztxRead.LoadRun();

                var channelNames = ztxRead.GetChannelsNames();
                if (channelNames == null || channelNames.Length == 0)
                {
                    Console.WriteLine("  ⚠️ Keine Channels in den Daten gefunden.");
                    return OracleExitCode.DataNotFound;
                }

                Console.WriteLine($"  {channelNames.Length} Channel(s) verfügbar.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ Daten konnten nicht geladen werden: {ex.Message}");
                return OracleExitCode.DataNotFound;
            }
            finally
            {
                ztxRead.ReleaseObjects();
            }

            Console.WriteLine("[validate] Alle Prüfungen bestanden.");
            return OracleExitCode.Success;
        }

        /// <summary>
        /// Ermittelt die Run-Metadaten aus einer HED-Datei oder der KOMPASS-Datenbank.
        /// </summary>
        private async Task<RunMetadata?> ResolveMetadataAsync(string runPath)
        {
            var riderPath = Path.GetFullPath(Path.Combine(runPath, ".."));
            Console.WriteLine($"Rider Path: {riderPath}");

            if (!Directory.Exists(riderPath))
            {
                Console.WriteLine($"Rider Path existiert nicht: {riderPath}");
                return null;
            }

            // --- Versuch 1: .HED-Datei suchen ---
            var hedService = new HedFileService();
            var hedFilePath = hedService.FindAndPrepareHedFile(runPath, riderPath);

            if (hedFilePath != null)
            {
                Console.WriteLine("Metadaten werden aus .HED-Datei geladen.");
                return hedService.ReadMetadata(hedFilePath);
            }

            // --- Versuch 2: KOMPASS-Datenbank ---
            Console.WriteLine("Kein .HED-File im Rider- oder Runordner gefunden.");
            Console.WriteLine("Lade KOMPASS...");

            if (!Directory.Exists(_config.DatabaseDirectory))
            {
                Console.WriteLine($"Datenbankverzeichnis nicht gefunden: {_config.DatabaseDirectory}");
                return null;
            }

            var kompass = new KompassService();
            if (!await kompass.LoadAsync(_config.DatabaseFilePath))
            {
                return null;
            }

            var metadata = kompass.BuildMetadata(runPath, _browse, _config.TeamMappings);
            hedService.WriteHedFile(runPath, metadata);
            return metadata;
        }

        /// <summary>
        /// Findet gültige WinTAX4-Runs über die IMarelliDataBrowse API.
        /// </summary>
        private List<string> DiscoverRuns(string runPath)
        {
            if (_browse.IsRunLocation(runPath))
            {
                return [runPath];
            }

            var foundRuns = _browse.FindRuns(runPath);
            return foundRuns?.ToList() ?? [];
        }
    }
}
