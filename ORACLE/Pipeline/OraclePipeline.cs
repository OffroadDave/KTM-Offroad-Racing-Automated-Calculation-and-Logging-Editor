using KtmDataHandler.Marelli.Wintax;
using KtmDataHandler.Models.Channels;
using Ktm.Racing.Domain.Geo;

namespace Ktm.Racing.Oracle
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

        // In diesem Aufruf reparierte Runs (für Status/Log).
        private readonly List<RunRepairResult> _repairs = [];

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
        public async Task<OracleExitCode> ProcessRunAsync(string runPath, bool force = false, bool gpsSplit = false)
        {
            Console.WriteLine($"[process] Run Path: {runPath}");

            if (!Directory.Exists(runPath))
            {
                Console.WriteLine($"Der angegebene Run Path existiert nicht: {runPath}");
                return OracleExitCode.DataNotFound;
            }

            // Unterbrochene GPS-Lap-Teilung zuerst (unter Geräte- und Run-Sperre) zurückrollen.
            if (RecoverInterruptedSplits(runPath) is { } recoveryExit)
                return recoveryExit;

            // Parallele ORACLE-Jobs dürfen nicht gleichzeitig denselben Run bearbeiten.
            using var processingLock = ProcessingLock.TryAcquire(runPath);
            if (processingLock is null)
            {
                Console.WriteLine($"[process] {runPath} wird gerade von einem anderen ORACLE-Prozess verarbeitet – später erneut versuchen.");
                return OracleExitCode.ResourceLocked;
            }

            // --- Idempotenz-Prüfung ---
            if (!force && OracleStatus.IsAlreadyProcessed(runPath))
            {
                var lengthCheck = CheckProcessedTrackLength([runPath], runPath);
                if (lengthCheck is { } code)
                {
                    Console.WriteLine("[process] Run bereits erfolgreich verarbeitet (oracle.status.json). Überspringe.");
                    Console.WriteLine("[process] Verwende --force um erneut zu verarbeiten.");
                    return code;
                }
            }

            var status = OracleStatus.Create(runPath);

            if (!_browse.IsRunLocation(runPath))
            {
                Console.WriteLine($"Warnung: Der Pfad ist kein gültiger WinTAX4-Run: {runPath}");
            }

            // --- Metadaten ermitteln ---
            RecordInputs(status);
            var (metadata, kompassInvalid) = await ResolveMetadataAsync(runPath);
            if (metadata == null)
            {
                var code = kompassInvalid ? OracleExitCode.InvalidData : OracleExitCode.DataNotFound;
                Console.WriteLine("Metadaten konnten nicht ermittelt werden.");
                status.SetStep("metadata", StepStatus.Failed);
                status.ExitCode = (int)code;
                status.Save(runPath);
                return code;
            }
            SetMetadataStep(status, metadata);

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

            // Track-Länge vorab aus KOMPASS/CIRCUITS/.HED/Vorschlägen bestimmen (MotoDB vergleicht exakt).
            var lengthGate = TrackLengthGate.FromConfig(_config);
            var knownLength = ResolveKnownTrackLength(lengthGate, metadata);

            // .HED in jedem Run-Ordner sicherstellen (kanonischer Name, normalisierter Inhalt).
            new HedFileService().EnsureHedInAllRuns(validRuns, Path.GetDirectoryName(runPath)!, metadata);

            // Optional: Runs per GPS-Ziellinie in Laps aufteilen (Flag --gps-split) vor den Berechnungen.
            if (gpsSplit)
                await RunGpsLapSplitAsync(validRuns, metadata, status);

            // --- Berechnungen (sVehicle + per-Lap-Distanzen) und XML-Properties je Run ---
            var xmlEditor = new XmlRunEditor();
            var importCalculator = new ImportDataCalculator(_config.Gps);
            var lapDistances = new List<IReadOnlyList<double>>();
            int calcSuccess = 0;
            foreach (var run in validRuns)
            {
                try
                {
                    var calc = importCalculator.RunCalculationPipeline(run);
                    lapDistances.Add(calc.LapDistancesMeters);
                    xmlEditor.SetRunProperties(run, metadata.BikeName, metadata.TrackName,
                        metadata.TrackLength, metadata.RiderName, lapDistances: calc.LapDistancesMeters);
                    calcSuccess++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[process] Fehler bei {Path.GetFileName(run)}: {ex.Message}");
                }
            }
            var calcStatus = calcSuccess == validRuns.Count ? StepStatus.Success
                           : calcSuccess > 0 ? StepStatus.Partial
                           : StepStatus.Failed;
            status.SetStep("properties", calcStatus);
            status.SetStep("calculations", calcStatus, new Dictionary<string, object>
            {
                ["processedRuns"] = calcSuccess,
                ["totalRuns"] = validRuns.Count,
            });

            // Ohne eine einzige erfolgreiche Berechnung ist der Run nicht verarbeitet: kein "Done", kein MotoDB-Start.
            if (calcSuccess == 0)
            {
                Console.WriteLine("[process] Berechnungen für keinen Run erfolgreich – Status bleibt offen (erneuter Versuch beim nächsten Aufruf).");
                status.ExitCode = (int)OracleExitCode.GeneralError;
                status.Save(runPath);
                return OracleExitCode.GeneralError;
            }

            // --- Track-Länge prüfen/schätzen: ohne eindeutige Länge darf MotoDB nicht laufen ---
            var lengthExit = FinishTrackLength(lengthGate, knownLength, metadata, validRuns,
                Path.GetDirectoryName(runPath)!, lapDistances, runPath, status);
            if (lengthExit != OracleExitCode.Success)
            {
                status.ExitCode = (int)lengthExit;
                status.Save(runPath);
                return lengthExit;
            }

            // --- Status-Datei schreiben ---
            var processExit = calcSuccess == validRuns.Count ? OracleExitCode.Success : OracleExitCode.PartialSuccess;
            status.ExitCode = (int)processExit;
            status.Save(runPath);

            Console.WriteLine("[process] Abgeschlossen.");
            return processExit;
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

            // --- Daten ladbar prüfen (nicht ladbar = beschädigt → InvalidData, kein Retry) ---
            var ztxRead = KtmDataHandlerFactory.CreateReader();
            try
            {
                ztxRead.SetDataLocation([runPath]);
                var load = ztxRead.LoadRun();
                if (load <= 0)
                {
                    Console.WriteLine($"  ❌ Daten konnten nicht geladen werden (LoadRun {load}).");
                    return OracleExitCode.InvalidData;
                }

                var channelNames = ztxRead.GetChannelsNames();
                if (channelNames == null || channelNames.Length == 0)
                {
                    Console.WriteLine("  ⚠️ Keine Channels in den Daten gefunden.");
                    return OracleExitCode.InvalidData;
                }

                Console.WriteLine($"  {channelNames.Length} Channel(s) verfügbar.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ Daten konnten nicht geladen werden: {ex.Message}");
                return OracleExitCode.InvalidData;
            }
            finally
            {
                ztxRead.ReleaseObjects();
            }

            Console.WriteLine("[validate] Alle Prüfungen bestanden.");
            return OracleExitCode.Success;
        }

        /// <summary>
        /// Erkennt, welche Runs eines Ordners tatsächlich Rennen (Renntempo) sind — im Gegensatz
        /// zu Motor-Blips, Standgas, langsamem Rollen, Sichtungsrunden oder Auslauf.
        /// Read-only: verändert keine Daten. Delegiert an <see cref="RaceSegmentDetector"/>.
        /// </summary>
        /// <param name="folderPath">Rider-/Session-Ordner oder direkter Run-Ordner.</param>
        /// <summary>Erkennt gültige Starts (Gatterstart) in allen Runs eines Pfads (read-only, siehe <see cref="StartDetector"/>).</summary>
        public OracleExitCode DetectStarts(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Der angegebene Pfad existiert nicht: {path}");
                return OracleExitCode.DataNotFound;
            }
            return new StartAnalyzer(_browse).Run(path);
        }

        public OracleExitCode DetectRaces(string folderPath)
        {
            Console.WriteLine($"[detect] Path: {folderPath}");

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"Der angegebene Pfad existiert nicht: {folderPath}");
                return OracleExitCode.DataNotFound;
            }

            var detector = new RaceSegmentDetector(_browse, _config.RaceDetection, _config.Gps);
            var result = detector.DetectRaces(folderPath);

            if (result.Runs.Count == 0)
            {
                Console.WriteLine("[detect] Keine Runs gefunden.");
                return OracleExitCode.DataNotFound;
            }

            foreach (var f in result.Runs)
            {
                string verdict = f.Verdict switch
                {
                    RaceRunVerdict.Race => "RENNEN",
                    RaceRunVerdict.Error => "FEHLER",
                    _ => "-",
                };
                Console.WriteLine(
                    $"  {f.RunFolderName,-12} dist={f.GpsDistanceMeters,7:F0} m  vGPS={f.MeanGpsSpeedKph,5:F1} kph  " +
                    $"Gas={f.MeanThrottlePercent,5:F1} %  rpm={f.MeanEngineRpm,6:F0}  " +
                    $"=> {verdict}{(f.LowConfidence ? " (review)" : "")}  [{f.Reason}]");
            }

            var raceNames = result.RaceRunPaths.Select(Path.GetFileName);
            Console.WriteLine($"[detect] Als Rennen erkannt: {(result.RaceCount > 0 ? string.Join(", ", raceNames) : "(keine)")}");
            return OracleExitCode.Success;
        }

        /// <summary>
        /// Routet einen <c>process</c>-Aufruf: einzelner Run → per-Run-Pipeline (KRAKEN-kompatibel),
        /// Rider-/Session-Ordner → renn-bewusste, durchgehende Verarbeitung (<see cref="ProcessRiderAsync"/>).
        /// </summary>
        public async Task<OracleExitCode> ProcessPathAsync(string path, bool force = false, bool gpsSplit = false)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Der angegebene Pfad existiert nicht: {path}");
                return OracleExitCode.DataNotFound;
            }

            // Geräteordner-Namen normalisieren, BEVOR KtmDataHandler den Pfad anfasst:
            // dessen native Verzeichnis-Handles (IsRunLocation/FindRuns) würden ein späteres
            // Directory.Move sonst mit "Access denied" blockieren.
            if (_config.NormalizeDeviceFolderName)
                path = NormalizeDeviceFolders(path);

            // Unterbrochene GPS-Lap-Teilungen zuerst (unter Sperre) zurückrollen – vor Reparatur, Erkennung und Idempotenz.
            if (RecoverInterruptedSplits(path) is { } recoveryExit)
                return recoveryExit;

            // Aus demselben Grund vor der WinTAX-API: verwaiste Lap-Ordner (Split-/Append-Reste ohne Daten) aus den
            // Runs räumen, damit WinTAX die Runs wieder erkennt und sie verarbeitet werden.
            if (_config.RepairOrphanLapFolders)
                _repairs.AddRange(RunRepair.RepairAll(path));

            return _browse.IsRunLocation(path)
                ? await ProcessRunAsync(path, force, gpsSplit)
                : await ProcessRiderAsync(path, force, gpsSplit);
        }

        /// <summary>
        /// Verarbeitet einen ganzen Rider-/Session-Ordner renn-bewusst (Option C):
        /// Die als Rennen erkannten Runs erhalten durchgehende <c>sVehicle</c>-Distanz und
        /// fortlaufende Lap-Nummerierung; Nicht-Renn-Runs werden eigenständig verarbeitet.
        /// Die Original-Ordnerstruktur bleibt unverändert.
        /// </summary>
        public async Task<OracleExitCode> ProcessRiderAsync(string riderPath, bool force = false, bool gpsSplit = false)
        {
            Console.WriteLine($"[process-rider] Rider Path: {riderPath}");

            // Geräteordner mit Leerzeichen kanonisch umbenennen ("AA 01" → "AA01"), damit der
            // bikeName in Ordnern und XML durchgängig ohne Leerzeichen ist. Non-fatal + guarded.
            if (_config.NormalizeDeviceFolderName)
                riderPath = NormalizeDeviceFolders(riderPath);

            if (RecoverInterruptedSplits(riderPath) is { } recoveryExit)
                return recoveryExit;

            var runs = (_browse.FindRuns(riderPath) ?? Enumerable.Empty<string>())
                .OrderBy(p => Path.GetFileName(p), StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Run-Ordner, die die WinTAX-API nicht erkennt (beschädigt), nie stillschweigend übergehen.
            var skipped = RunFolderInspector.FindSkipped(riderPath, runs);

            // Mehrere Geräteordner (z.B. Session-Ordner "BC" mit AA01 + AA02): jeden Geräteordner eigenständig
            // verarbeiten – mit eigenen Metadaten (Bike/Rider), eigener .HED und eigenem Status.
            var devices = runs.Concat(skipped.Select(s => s.RunPath))
                .Select(Path.GetDirectoryName)
                .Where(d => !string.IsNullOrEmpty(d))
                .Select(d => d!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(d => d, StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (devices.Count > 1)
            {
                Console.WriteLine($"[process-rider] {devices.Count} Geräteordner gefunden – jeder wird einzeln verarbeitet: " +
                                  string.Join(", ", devices.Select(Path.GetFileName)));
                var codes = new List<OracleExitCode>();
                foreach (var device in devices)
                {
                    var code = await ProcessRiderAsync(device, force, gpsSplit);
                    Console.WriteLine($"[process-rider] {Path.GetFileName(device)}: Exit-Code {(int)code} ({code})");
                    codes.Add(code);
                }
                return OracleExitCodes.Combine(codes);
            }

            var corrupt = skipped.Where(s => !s.IsEmpty).ToList();
            var empty = skipped.Where(s => s.IsEmpty).ToList();
            foreach (var s in corrupt)
                Console.WriteLine($"[discovery] ⚠️ Run übersprungen (kein gültiger WinTAX4-Run): {s.RunPath} – {s.Reason}");
            foreach (var s in empty)
                Console.WriteLine($"[discovery] Leerer Run (keine Lap-Daten aufgezeichnet): {Path.GetFileName(s.RunPath)} – es werden nur die Metadaten gesetzt.");
            var skippedDetails = corrupt.Select(s => $"{Path.GetFileName(s.RunPath)}: {s.Reason}").ToList();
            var emptyRuns = empty.Select(s => s.RunPath).ToList();
            var deviceFolder = devices.FirstOrDefault() ?? riderPath;
            var repairedDetails = _repairs
                .Where(r => string.Equals(Path.GetDirectoryName(r.RunPath), deviceFolder, StringComparison.OrdinalIgnoreCase))
                .Select(r => $"{Path.GetFileName(r.RunPath)}: {r.MovedLaps.Count} orphaned lap folder(s) moved to {RunRepair.RemovedFolderName}")
                .ToList();

            if (runs.Count == 0)
            {
                var s = OracleStatus.Create(riderPath);
                if (corrupt.Count == 0 && empty.Count > 0)
                {
                    // Nur leere Runs: keine Messdaten, kein Fehler.
                    Console.WriteLine($"[process-rider] {riderPath}: nur leere Runs (keine Lap-Daten) – nichts zu berechnen.");
                    s.SetStep("discovery", StepStatus.Skipped, new Dictionary<string, object> { ["emptyRuns"] = emptyRuns.Select(Path.GetFileName).ToList() });
                    s.ExitCode = (int)OracleExitCode.Success;
                    s.Save(riderPath);
                    return OracleExitCode.Success;
                }

                var code = corrupt.Count > 0 ? OracleExitCode.InvalidData : OracleExitCode.DataNotFound;
                Console.WriteLine(corrupt.Count > 0
                    ? $"Keine verarbeitbaren Runs in: {riderPath} – {corrupt.Count} beschädigte(r) Run-Ordner (siehe oben). In WinTAX reparieren, dann mit --force erneut ausführen."
                    : $"Keine gültigen Runs gefunden in: {riderPath}");
                s.SetStep("discovery", StepStatus.Failed, corrupt.Count > 0
                    ? new Dictionary<string, object> { ["skippedRuns"] = skippedDetails }
                    : null);
                s.ExitCode = (int)code;
                s.Save(riderPath);
                return code;
            }

            // Status/Idempotenz gehören in den Geräteordner (Elternordner der Runs, z.B. "…\AA01"),
            // nicht in einen ggf. übergeordneten Session-Ordner (z.B. "…\R1").
            var statusFolder = DetermineStatusFolder(riderPath, runs);

            // Parallele ORACLE-Jobs (KRAKEN) dürfen nicht gleichzeitig in denselben Geräteordner schreiben.
            using var processingLock = ProcessingLock.TryAcquire(statusFolder);
            if (processingLock is null)
            {
                Console.WriteLine($"[process-rider] {statusFolder} wird gerade von einem anderen ORACLE-Prozess verarbeitet – später erneut versuchen.");
                return OracleExitCode.ResourceLocked;
            }

            if (!force && OracleStatus.IsAlreadyProcessed(statusFolder))
            {
                var lengthCheck = CheckProcessedTrackLength(runs, statusFolder);
                if (lengthCheck is { } code)
                {
                    Console.WriteLine("[process-rider] Bereits verarbeitet (oracle.status.json). Überspringe. (--force zum Erzwingen)");
                    return code == OracleExitCode.Success && corrupt.Count > 0 ? OracleExitCode.PartialSuccess : code;
                }
            }

            var status = OracleStatus.Create(statusFolder);
            status.SetStep("discovery", corrupt.Count > 0 ? StepStatus.Partial : StepStatus.Success, new Dictionary<string, object>
            {
                ["validRuns"] = runs.Count,
                ["skippedRuns"] = skippedDetails,
                ["emptyRuns"] = emptyRuns.Select(Path.GetFileName).ToList(),
                ["repairedRuns"] = repairedDetails,
            });
            RecordInputs(status);

            // Metadaten einmalig über einen repräsentativen Run ermitteln (alle Runs gehören zu genau einem Gerät).
            var (metadata, kompassInvalid) = await ResolveMetadataAsync(runs[0]);
            if (metadata == null)
            {
                var code = kompassInvalid ? OracleExitCode.InvalidData : OracleExitCode.DataNotFound;
                Console.WriteLine("Metadaten konnten nicht ermittelt werden.");
                status.SetStep("metadata", StepStatus.Failed);
                status.ExitCode = (int)code;
                status.Save(statusFolder);
                return code;
            }
            SetMetadataStep(status, metadata);

            // Track-Länge vorab bestimmen (auch für die relative Distanzschwelle der Renn-Erkennung).
            var lengthGate = TrackLengthGate.FromConfig(_config);
            var knownLength = ResolveKnownTrackLength(lengthGate, metadata);

            // .HED in jedem Run-Ordner sicherstellen (kanonischer Name, normalisierter Inhalt) – auch in leeren Runs.
            var allRuns = runs.Concat(emptyRuns).ToList();
            new HedFileService().EnsureHedInAllRuns(allRuns, statusFolder, metadata);
            SetEmptyRunProperties(emptyRuns, metadata);

            // Optional: Runs per GPS-Ziellinie in Laps aufteilen (Flag --gps-split), vor der Stint-Verarbeitung.
            if (gpsSplit)
                await RunGpsLapSplitAsync(runs, metadata, status);

            double trackLength = ParseTrackLength(metadata.TrackLength);

            var stintProcessor = new RaceStintProcessor(
                new RaceSegmentDetector(_browse, _config.RaceDetection, _config.Gps));
            var result = stintProcessor.ProcessRider(riderPath, metadata, trackLength);

            status.SetStep("raceDetection", StepStatus.Success, new Dictionary<string, object>
            {
                ["raceRuns"] = result.RaceRunCount,
                ["nonRaceRuns"] = result.NonRaceRunCount,
                ["totalRuns"] = runs.Count,
            });
            status.SetStep("stint", result.FailedRuns.Count > 0 || result.RaceRunCount == 0 ? StepStatus.Partial : StepStatus.Success,
                new Dictionary<string, object>
                {
                    ["totalRaceLaps"] = result.TotalRaceLaps,
                    ["totalRaceDistanceMeters"] = Math.Round(result.TotalRaceDistanceMeters, 1),
                    ["failedRuns"] = result.FailedRuns,
                });

            // Ohne eine einzige erfolgreiche Berechnung ist der Fahrer nicht verarbeitet: kein "Done"/"LengthPending", kein MotoDB-Start.
            if (result.ProcessedRunPaths.Count == 0 && result.FailedRuns.Count > 0)
            {
                Console.WriteLine("[process-rider] Berechnungen für keinen Run erfolgreich – Status bleibt offen (erneuter Versuch beim nächsten Aufruf).");
                status.ExitCode = (int)OracleExitCode.GeneralError;
                status.Save(statusFolder);
                return OracleExitCode.GeneralError;
            }

            // --- Track-Länge prüfen/schätzen: ohne eindeutige Länge darf MotoDB nicht laufen ---
            var lengthExit = FinishTrackLength(lengthGate, knownLength, metadata, allRuns, statusFolder,
                result.LapDistancesPerRun, statusFolder, status);
            if (lengthExit != OracleExitCode.Success)
            {
                status.ExitCode = (int)lengthExit;
                status.Save(statusFolder);
                return lengthExit;
            }

            var exit = corrupt.Count > 0 || result.FailedRuns.Count > 0 ? OracleExitCode.PartialSuccess
                : OracleExitCode.Success;
            status.ExitCode = (int)exit;
            status.Save(statusFolder);

            Console.WriteLine(
                $"[process-rider] Abgeschlossen: {result.RaceRunCount} Renn-Run(s), " +
                $"{result.TotalRaceLaps} Renn-Laps, {result.TotalRaceDistanceMeters:F0} m Gesamtdistanz." +
                (repairedDetails.Count > 0 ? $" {repairedDetails.Count} Run(s) repariert." : string.Empty) +
                (empty.Count > 0 ? $" {empty.Count} leere(r) Run(s) (nur Metadaten)." : string.Empty) +
                (corrupt.Count > 0 ? $" {corrupt.Count} beschädigte(r) Run-Ordner NICHT verarbeitet." : string.Empty) +
                (result.FailedRuns.Count > 0 ? $" {result.FailedRuns.Count} Run(s) fehlgeschlagen." : string.Empty));
            return exit;
        }

        /// <summary>Setzt in leeren Runs (keine Lap-Daten) die Run-Metadaten (Car.xml, Session.xml), damit sie konsistent benannt sind.</summary>
        private static void SetEmptyRunProperties(IReadOnlyList<string> emptyRuns, RunMetadata metadata)
        {
            var xml = new XmlRunEditor();
            foreach (var run in emptyRuns)
            {
                try
                {
                    xml.SetRunProperties(run, metadata.BikeName, metadata.TrackName, metadata.TrackLength, metadata.RiderName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[process-rider] ⚠️ {Path.GetFileName(run)}: Metadaten konnten nicht geschrieben werden – {ex.Message}");
                }
            }
        }

        /// <summary>Schreibt den Metadaten-Schritt; ohne aufgelösten Rider (KOMPASS) ist er nur teilweise erfolgreich.</summary>
        private static void SetMetadataStep(OracleStatus status, RunMetadata metadata)
        {
            var details = new Dictionary<string, object>
            {
                ["bikeName"] = metadata.BikeName,
                ["riderName"] = metadata.RiderName,
                ["teamName"] = metadata.TeamName,
                ["seasonName"] = metadata.SeasonName,
                ["disciplineName"] = metadata.DisciplineName,
                ["trackName"] = metadata.TrackName,
            };
            var complete = !string.IsNullOrWhiteSpace(metadata.RiderName) && !string.IsNullOrWhiteSpace(metadata.TrackName);
            if (!complete)
                Console.WriteLine("[metadata] ⚠️ Metadaten unvollständig (Rider oder Track nicht aufgelöst) – in KODE pflegen.");
            status.SetStep("metadata", complete ? StepStatus.Success : StepStatus.Partial, details);
        }

        /// <summary>Protokolliert den Datenstand der Eingaben (KOMPASS, loadChannels) in Log und Status.</summary>
        private void RecordInputs(OracleStatus status)
        {
            var kompass = InputFingerprint.Describe(_config.DatabaseFilePath);
            var workbook = InputFingerprint.Describe(_config.MotoDbLoadChannelsPath);
            Console.WriteLine($"[inputs] KOMPASS: {kompass}");
            Console.WriteLine($"[inputs] loadChannels: {workbook}");
            status.SetStep("inputs", StepStatus.Success, new Dictionary<string, object>
            {
                ["kompass"] = kompass,
                ["loadChannels"] = workbook,
            });
        }

        /// <summary>
        /// Bestimmt den Ordner für die <c>oracle.status.json</c>: den Geräteordner (gemeinsamer
        /// Elternordner der Runs, z.B. "…\AA01"). Nur wenn die Runs über mehrere Geräteordner
        /// verteilt sind, wird auf <paramref name="riderPath"/> zurückgefallen.
        /// </summary>
        private static string DetermineStatusFolder(string riderPath, IReadOnlyList<string> runs)
        {
            var deviceFolders = runs
                .Select(Path.GetDirectoryName)
                .Where(d => !string.IsNullOrEmpty(d))
                .Select(d => d!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            return deviceFolders.Count == 1 ? deviceFolders[0] : riderPath;
        }

        /// <summary>
        /// Parst die Track-Länge (String aus Metadaten) zu Metern wie die Track-Längen-Prüfung (ganze Meter, z.B. "1592");
        /// 0 wenn unbekannt oder ungültig ("1850,5" oder "1.850" werden nicht als 18505 bzw. 1,85 m gelesen).
        /// </summary>
        internal static double ParseTrackLength(string trackLength)
            => Ktm.Racing.Domain.Kompass.TrackLengthRules.Parse(trackLength) ?? 0.0;

        /// <summary>
        /// Bestimmt die Track-Länge vor den Berechnungen aus KOMPASS, CIRCUITS, .HED oder offenen Vorschlägen
        /// und übernimmt sie in die Metadaten (für .HED, Session.xml und Renn-Erkennung).
        /// </summary>
        private static TrackLengthDecision ResolveKnownTrackLength(TrackLengthGate gate, RunMetadata metadata)
        {
            var known = gate.ResolveKnown(metadata.TrackName, metadata.TrackLength);
            if (known.LengthMeters is not null)
                metadata.TrackLength = known.LengthText;
            Console.WriteLine($"[trackLength] {known.Message}");
            return known;
        }

        /// <summary>
        /// Schließt die Track-Längen-Prüfung ab (ggf. Schätzung aus den Rundendistanzen), schreibt eine geänderte
        /// Länge in .HED und Session.xml aller Runs und liefert den Exit-Code: <see cref="OracleExitCode.Success"/>,
        /// <see cref="OracleExitCode.ResourceLocked"/> (loadChannels.xlsx gesperrt) oder
        /// <see cref="OracleExitCode.TrackLengthMissing"/> (keine eindeutige Länge, MotoDB darf nicht laufen).
        /// </summary>
        private OracleExitCode FinishTrackLength(TrackLengthGate gate, TrackLengthDecision known, RunMetadata metadata,
            IReadOnlyList<string> runs, string hedFolder, IEnumerable<IReadOnlyList<double>> lapDistances,
            string origin, OracleStatus status)
        {
            // Reichen die eigenen Runden nicht, werden die Runden bereits verarbeiteter Runs desselben Events genutzt.
            var eventFolder = runs.Count > 0 ? LapHistory.FindEventFolder(runs[0], metadata.EventName) : null;
            Func<IEnumerable<IReadOnlyList<double>>>? eventLaps = eventFolder is null
                ? null
                : () => LapHistory.ReadProcessedLapDistances(eventFolder, runs);

            var decision = gate.Complete(known, metadata, lapDistances, origin, eventLaps);
            foreach (var warning in decision.Warnings)
                Console.WriteLine($"[trackLength] ⚠️ {warning}");
            Console.WriteLine($"[trackLength] {decision.Status}: {decision.Message}");

            var data = new Dictionary<string, object>
            {
                ["source"] = decision.Source.ToString(),
                ["provisional"] = decision.Provisional,
            };
            if (decision.LengthMeters is { } meters)
                data["lengthMeters"] = meters;

            switch (decision.Status)
            {
                case TrackLengthGateStatus.Locked:
                    status.SetStep("trackLength", StepStatus.Failed, data);
                    return OracleExitCode.ResourceLocked;

                case TrackLengthGateStatus.Missing:
                    status.SetStep("trackLength", StepStatus.Failed, data);
                    if (!_config.TrackLength.Enforce)
                        return OracleExitCode.Success;
                    Console.WriteLine("[trackLength] Keine eindeutige Track-Länge – MotoDB wird nicht gestartet. Länge in KODE eintragen und erneut ausführen.");
                    return OracleExitCode.TrackLengthMissing;
            }

            if (!string.Equals(decision.LengthText, metadata.TrackLength, StringComparison.Ordinal))
            {
                metadata.TrackLength = decision.LengthText;
                new HedFileService().EnsureHedInAllRuns(runs, hedFolder, metadata);
                var xml = new XmlRunEditor();
                foreach (var run in runs)
                    xml.SetTrackLength(run, metadata.TrackLength);
            }

            status.SetStep("trackLength", StepStatus.Success, data);
            return OracleExitCode.Success;
        }

        /// <summary>
        /// Prüft bei bereits verarbeiteten Runs (Idempotenz-Skip) die Track-Länge: ist sie bekannt – oder aus den bereits
        /// geschriebenen Rundendistanzen (eigene Runs + Event) schätzbar –, werden .HED, Session.xml und CIRCUITS bei
        /// Abweichung nachgezogen und der Status aktualisiert (auch nach Exit 5/6: keine Vollverarbeitung nötig).
        /// <c>null</c> = keine Länge und keine gespeicherten Rundendistanzen → vollständig neu verarbeiten.
        /// </summary>
        private OracleExitCode? CheckProcessedTrackLength(IReadOnlyList<string> runs, string origin)
        {
            var hedFile = runs
                .Where(Directory.Exists)
                .Select(r => Directory.EnumerateFiles(r, "*.HED", SearchOption.TopDirectoryOnly).FirstOrDefault())
                .FirstOrDefault(f => f is not null);
            if (hedFile is null)
            {
                Console.WriteLine("[trackLength] Bereits verarbeitet, aber keine .HED gefunden – verarbeite erneut.");
                return null;
            }

            var hed = new HedFileService();
            var metadata = hed.ReadMetadata(hedFile);
            if (string.IsNullOrEmpty(metadata.EventName) && runs.Count > 0)
                metadata.EventName = _browse.GetEventName(runs[0]) ?? "";
            var gate = TrackLengthGate.FromConfig(_config);
            var known = gate.ResolveKnown(metadata.TrackName, metadata.TrackLength);
            var previous = OracleStatus.Load(origin);

            var ownLaps = LapHistory.ReadLapDistances(runs);
            if (known.Status != TrackLengthGateStatus.Ok && ownLaps.Count == 0 && previous?.ExitCode is not (5 or 6))
            {
                Console.WriteLine($"[trackLength] {known.Message} Bereits verarbeitet, aber ohne Länge und ohne Rundendistanzen – verarbeite erneut.");
                return null;
            }

            var eventFolder = runs.Count > 0 ? LapHistory.FindEventFolder(runs[0], metadata.EventName) : null;
            var decision = gate.Complete(known, metadata, ownLaps, origin,
                eventFolder is null ? null : () => LapHistory.ReadProcessedLapDistances(eventFolder, runs));
            foreach (var warning in decision.Warnings)
                Console.WriteLine($"[trackLength] ⚠️ {warning}");
            if (decision.Status == TrackLengthGateStatus.Locked)
                return OracleExitCode.ResourceLocked;
            if (decision.Status == TrackLengthGateStatus.Missing)
            {
                Console.WriteLine($"[trackLength] {decision.Message}");
                return _config.TrackLength.Enforce ? OracleExitCode.TrackLengthMissing : OracleExitCode.Success;
            }

            var changed = !string.Equals(decision.LengthText, metadata.TrackLength, StringComparison.Ordinal);
            if (changed)
            {
                Console.WriteLine($"[trackLength] Länge geändert: {(metadata.TrackLength.Length == 0 ? "(leer)" : metadata.TrackLength)} → {decision.LengthText} m ({decision.Source}).");
                var xml = new XmlRunEditor();
                foreach (var run in runs)
                {
                    hed.SetTrackLength(run, decision.LengthText);
                    xml.SetTrackLength(run, decision.LengthText);
                }
            }

            // Korrektur / nachgeholte Länge im Status nachvollziehbar festhalten.
            if (previous is not null && (changed || previous.ExitCode is 5 or 6))
            {
                var details = new Dictionary<string, object>
                {
                    ["source"] = decision.Source.ToString(),
                    ["provisional"] = decision.Provisional,
                    ["lengthMeters"] = decision.LengthMeters!.Value,
                    ["correctedAtUtc"] = DateTime.UtcNow,
                };
                if (changed && metadata.TrackLength.Length > 0)
                    details["correctedFrom"] = metadata.TrackLength;
                previous.SetStep("trackLength", StepStatus.Success, details);
                if (previous.ExitCode is 5 or 6)
                {
                    var discoveryPartial = previous.Steps.TryGetValue("discovery", out var d) && d.Status == StepStatus.Partial;
                    previous.ExitCode = (int)(discoveryPartial ? OracleExitCode.PartialSuccess : OracleExitCode.Success);
                    Console.WriteLine("[trackLength] Länge nachgeholt – Status auf erfolgreich gesetzt (keine Vollverarbeitung nötig).");
                }
                previous.Save(origin);
            }

            // Untergrund aus KOMPASS nachziehen, falls er sich geändert hat.
            var surface = gate.KompassSurface(metadata.TrackName);
            if (surface.Length > 0 && !string.Equals(surface, metadata.Surface, StringComparison.Ordinal))
            {
                Console.WriteLine($"[hed] Untergrund geändert: \"{metadata.Surface}\" → \"{surface}\".");
                foreach (var run in runs)
                    hed.SetTrackSurface(run, surface);
            }

            return OracleExitCode.Success;
        }

        /// <summary>
        /// Teilt die angegebenen Runs per GPS-Ziellinie in Laps (<see cref="GpsLapSplitService"/>): eine Ziellinie für alle Runs
        /// des Tracks aus der KOMPASS-Koordinate, Erkennung bereits geteilter Runs, Schreiben mit Sicherung und Rollback.
        /// Non-fatal: ohne Ziellinie oder GPS wird übersprungen, ein Fehler stoppt die Pipeline nicht.
        /// </summary>
        private async Task<IReadOnlyList<GpsLapSplitOutcome>> RunGpsLapSplitAsync(IReadOnlyList<string> runs, RunMetadata metadata,
            OracleStatus? status, bool dryRun = false, bool lockRuns = false)
        {
            var reference = await ResolveFinishLineAsync(metadata);
            if (reference is not { } point)
            {
                Console.WriteLine($"[gps-split] Übersprungen: keine Koordinate in KOMPASS für Track \"{metadata.TrackName}\" – Lat/Lon in KODE eintragen.");
                status?.SetStep("gpsSplit", StepStatus.Skipped, new Dictionary<string, object> { ["reason"] = "no track coordinate in KOMPASS" });
                return [];
            }

            // Status vor jedem Lap-Tausch (unter der Run-Sperre) invalidieren: geänderte Laps (Lap-Nummern, LapDistance,
            // sVehicle) müssen neu berechnet werden, auch wenn ORACLE direkt nach dem Tausch abbricht.
            var service = new GpsLapSplitService(_config.GpsLapSplit, lockRuns: lockRuns, gps: _config.Gps,
                beforeSwap: run => InvalidateProcessedStatus(run, "laps split by GPS finish line"));
            var (line, outcomes) = service.SplitRuns(runs, point, dryRun);
            Console.WriteLine(line.Line is { } l
                ? $"[gps-split] Ziellinie {metadata.TrackName}: {line.Message} " +
                  $"Mitte {l.Center.Latitude:0.0000000}, {l.Center.Longitude:0.0000000}, Länge {l.LengthMeters:0} m."
                : $"[gps-split] Keine Ziellinie: {line.Message}");

            foreach (var outcome in outcomes)
            {
                var icon = outcome.Status switch
                {
                    GpsLapSplitOutcomeStatus.Failed => "❌ ",
                    GpsLapSplitOutcomeStatus.Split or GpsLapSplitOutcomeStatus.Planned => "✂ ",
                    _ => string.Empty,
                };
                Console.WriteLine($"[gps-split] {icon}{Path.GetFileName(outcome.RunPath)}: {outcome.Message}");
            }

            var split = outcomes.Count(o => o.Status is GpsLapSplitOutcomeStatus.Split or GpsLapSplitOutcomeStatus.Planned);
            var failed = outcomes.Count(o => o.Status == GpsLapSplitOutcomeStatus.Failed);
            var skipped = outcomes.Count - split - failed;
            Console.WriteLine($"[gps-split] Abgeschlossen{(dryRun ? " (Probelauf, nichts geschrieben)" : string.Empty)}: " +
                              $"{split} {(dryRun ? "zu teilen" : "geteilt")}, {skipped} übersprungen, {failed} Fehler.");

            status?.SetStep("gpsSplit", failed > 0 ? StepStatus.Partial : split == 0 ? StepStatus.Skipped : StepStatus.Success,
                new Dictionary<string, object>
                {
                    ["split"] = split,
                    ["skipped"] = skipped,
                    ["failed"] = failed,
                    ["finishLine"] = line.Message,
                    ["runs"] = outcomes.Select(o => $"{Path.GetFileName(o.RunPath)}: {o.Status} – {o.Message}").ToList(),
                });
            return outcomes;
        }

        /// <summary>
        /// Command <c>split-laps</c>: teilt alle Runs unter <paramref name="path"/> (Event-, Session-, Geräte- oder Run-Ordner)
        /// per GPS-Ziellinie in Laps; <paramref name="dryRun"/> zeigt nur, was passieren würde; <paramref name="undo"/> stellt
        /// die Original-Laps der letzten Teilung wieder her. Sperrt die Geräteordner während der Arbeit.
        /// </summary>
        public async Task<OracleExitCode> SplitLapsAsync(string path, bool dryRun, bool undo)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Der angegebene Pfad existiert nicht: {path}");
                return OracleExitCode.DataNotFound;
            }

            var runs = RunFolderInspector.FindRunFolders(path);
            if (runs.Count == 0)
            {
                Console.WriteLine($"[split-laps] Keine Run-Ordner gefunden in: {path}");
                return OracleExitCode.DataNotFound;
            }

            var locks = new List<ProcessingLock>();
            try
            {
                var lockedRuns = new List<string>();
                foreach (var device in runs.GroupBy(r => Path.GetDirectoryName(r)!, StringComparer.OrdinalIgnoreCase))
                {
                    if (ProcessingLock.TryAcquire(device.Key) is { } deviceLock)
                    {
                        locks.Add(deviceLock);
                        lockedRuns.AddRange(device);
                    }
                    else
                    {
                        Console.WriteLine($"[split-laps] {device.Key} wird gerade von einem anderen ORACLE-Prozess verarbeitet – übersprungen.");
                    }
                }

                if (lockedRuns.Count == 0)
                    return OracleExitCode.ResourceLocked;
                var lockedOut = lockedRuns.Count < runs.Count;

                // Unterbrochene Teilungen unter den Sperren zurückrollen, bevor Undo/Analyse den Run ansehen; der Probelauf
                // meldet sie nur (Analyze). Für --undo zählt eine Wiederherstellung als erfolgreich rückgängig gemacht.
                var recovery = undo || !dryRun ? LapSplitStore.RecoverInterruptedLocked(lockedRuns, deviceLocked: true) : null;
                var unrecovered = recovery is null ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    : recovery.Failed.Concat(recovery.Locked).ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (undo)
                {
                    int restored = 0, failedUndo = 0;
                    foreach (var run in lockedRuns)
                    {
                        if (recovery!.Recovered.Contains(run, StringComparer.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"[split-laps] {Path.GetFileName(run)}: ✔ interrupted lap split rolled back, original laps restored");
                            restored++;
                            try
                            {
                                InvalidateProcessedStatus(run, "interrupted lap split rolled back");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[split-laps] ⚠️ {Path.GetFileName(run)}: Verarbeitungsstatus konnte nicht zurückgesetzt werden: {ex.Message}");
                            }
                            continue;
                        }
                        if (unrecovered.Contains(run))
                        {
                            failedUndo++;
                            continue;
                        }

                        using var runLock = ProcessingLock.TryAcquire(run);
                        var result = runLock is null
                            ? new SplitUndoResult(false, "the run is being processed by another ORACLE process – try again later")
                            : UndoSplit(run);
                        Console.WriteLine($"[split-laps] {Path.GetFileName(run)}: {(result.Success ? "✔ " : string.Empty)}{result.Message}");
                        if (result.Success)
                            restored++;
                        else if (LapSplitStore.FindLatestSplit(run).Manifest is not null) failedUndo++;
                    }

                    Console.WriteLine($"[split-laps] Rückgängig: {restored} Run(s) wiederhergestellt, {failedUndo} Fehler.");
                    return failedUndo > 0 || lockedOut ? OracleExitCode.PartialSuccess : OracleExitCode.Success;
                }

                // Eine Ziellinie je Track: Runs nach Event-Ordner gruppieren. Track ohne Seiteneffekte aus dem Event-Ordner
                // (die volle Metadaten-Ermittlung legt ggf. .HED-Dateien an und wird nur ohne Probelauf als Rückfall genutzt).
                var outcomes = new List<GpsLapSplitOutcome>();
                var unresolved = 0;
                var kompassInvalid = false;
                foreach (var eventRuns in lockedRuns.GroupBy(r => KompassService.ParseEventFromPath(r)?.EventName ?? string.Empty, StringComparer.OrdinalIgnoreCase))
                {
                    var group = eventRuns.ToList();
                    var metadata = KompassService.ParseEventFromPath(group[0]);
                    if (metadata is null && !dryRun)
                    {
                        var resolved = await ResolveMetadataAsync(group[0]);
                        metadata = resolved.Metadata;
                        kompassInvalid |= resolved.KompassInvalid;
                    }
                    if (metadata is null || string.IsNullOrWhiteSpace(metadata.TrackName))
                    {
                        Console.WriteLine($"[split-laps] Track für {group.Count} Run(s) nicht ermittelbar (Event-Ordner im Format RT37_CHN-Shanghai erwartet), z.B. {group[0]}.");
                        unresolved += group.Count;
                        continue;
                    }

                    Console.WriteLine($"[split-laps] Track: {metadata.TrackName} ({metadata.CountryCode}), {group.Count} Run(s){(dryRun ? ", Probelauf" : string.Empty)}.");
                    var groupOutcomes = await RunGpsLapSplitAsync(group, metadata, null, dryRun, lockRuns: true);
                    if (groupOutcomes.Count == 0)
                        unresolved += group.Count;
                    outcomes.AddRange(groupOutcomes);
                }

                if (outcomes.Count == 0)
                    return kompassInvalid ? OracleExitCode.InvalidData : OracleExitCode.DataNotFound;

                var failed = outcomes.Count(o => o.Status == GpsLapSplitOutcomeStatus.Failed);
                return failed == outcomes.Count ? OracleExitCode.GeneralError
                    : failed > 0 || lockedOut || unresolved > 0 || unrecovered.Count > 0 ? OracleExitCode.PartialSuccess
                    : OracleExitCode.Success;
            }
            finally
            {
                foreach (var held in locks)
                    held.Dispose();
            }
        }

        /// <summary>
        /// Rollt unterbrochene GPS-Lap-Teilungen (ausstehendes Manifest) aller Runs unter <paramref name="root"/> unter Geräte- und
        /// Run-Sperre zurück, bevor ein schreibender Befehl Runs erkennt oder die Idempotenz prüft. <c>null</c>, wenn nichts
        /// aussteht; sonst der Exit-Code, mit dem abgebrochen wird (Originale bleiben in der Sicherung).
        /// </summary>
        private static OracleExitCode? RecoverInterruptedSplits(string root)
        {
            var recovery = LapSplitStore.RecoverInterruptedLocked(RunFolderInspector.FindRunFolders(root));
            if (recovery.Failed.Count > 0)
            {
                Console.WriteLine($"[process] Unterbrochene Lap-Teilung konnte nicht zurückgerollt werden ({string.Join(", ", recovery.Failed.Select(Path.GetFileName))}) – " +
                                  $"Originale in {LapSplitStore.BackupFolderName}, manuell wiederherstellen.");
                return OracleExitCode.GeneralError;
            }
            if (recovery.Locked.Count > 0)
            {
                Console.WriteLine($"[process] Unterbrochene Lap-Teilung in gesperrtem Run ({string.Join(", ", recovery.Locked.Select(Path.GetFileName))}) – später erneut versuchen.");
                return OracleExitCode.ResourceLocked;
            }

            return null;
        }

        /// <summary>
        /// Stellt die Original-Laps der letzten Teilung wieder her. Der Verarbeitungsstatus wird vorher (unter der Run-Sperre)
        /// invalidiert; lässt sich das nicht speichern, bleibt der Run unverändert.
        /// </summary>
        private static SplitUndoResult UndoSplit(string run)
        {
            if (LapSplitStore.FindLatestSplit(run).Manifest is null)
                return LapSplitStore.Undo(run);
            try
            {
                InvalidateProcessedStatus(run, "lap split undone");
            }
            catch (Exception ex)
            {
                return new SplitUndoResult(false, $"processing status could not be invalidated – nothing changed: {ex.Message}");
            }
            return LapSplitStore.Undo(run);
        }

        /// <summary>
        /// Markiert den Status des Runs und seines Geräteordners als veraltet, damit der nächste <c>process</c>-Aufruf die
        /// geänderten Laps neu berechnet (Lap-Nummern, LapDistance, sVehicle). Wirft, wenn die Invalidierung nicht
        /// gespeichert werden kann – der Aufrufer darf die Laps dann nicht tauschen.
        /// </summary>
        private static void InvalidateProcessedStatus(string runPath, string reason)
        {
            OracleStatus.MarkOutdated(runPath, reason);
            if (Path.GetDirectoryName(runPath) is { } device)
                OracleStatus.MarkOutdated(device, reason);
            Console.WriteLine($"[split-laps] {Path.GetFileName(runPath)}: Verarbeitungsstatus zurückgesetzt – nächster 'process'-Aufruf rechnet neu.");
        }

        /// <summary>
        /// Ermittelt die Track-Koordinate (Referenz für die Ziellinie) aus KOMPASS – schreibweisen-tolerant, Land bevorzugt.
        /// Gibt <c>null</c> zurück, wenn KOMPASS fehlt oder der Track keine Lat/Lon-Werte hat.
        /// </summary>
        private async Task<GeoCoordinate?> ResolveFinishLineAsync(RunMetadata metadata)
        {
            if (string.IsNullOrWhiteSpace(metadata.TrackName))
                return null;

            if (!Directory.Exists(_config.DatabaseDirectory))
            {
                Console.WriteLine($"[gps-split] KOMPASS-Verzeichnis nicht gefunden: {_config.DatabaseDirectory}");
                return null;
            }

            var kompass = new KompassService();
            if (!await kompass.LoadAsync(_config.DatabaseFilePath))
                return null;

            return kompass.FindFinishLineByTrack(metadata.TrackName, metadata.CountryCode);
        }

        /// <summary>
        /// Ergänzt fehlende Track-Metadaten (aktuell: Untergrund/Surface) aus KOMPASS, falls im .HED nicht
        /// vorhanden. Lädt KOMPASS nur bei Bedarf (Surface leer und Trackname bekannt). Non-fatal.
        /// </summary>
        private async Task EnrichTrackMetadataAsync(RunMetadata metadata)
        {
            if (!string.IsNullOrWhiteSpace(metadata.Surface) || string.IsNullOrWhiteSpace(metadata.TrackName))
                return;
            if (!Directory.Exists(_config.DatabaseDirectory))
                return;

            var kompass = new KompassService();
            if (!await kompass.LoadAsync(_config.DatabaseFilePath))
                return;

            metadata.Surface = kompass.FindTrackSurface(metadata.TrackName) ?? "";
        }

        /// <summary>
        /// Benennt Geräteordner mit Leerzeichen im Namen kanonisch um (z.B. <c>"AA 01"</c> → <c>"AA01"</c>),
        /// damit der bikeName in Ordnern und XML durchgängig ohne Leerzeichen ist.
        /// Erkennt Geräteordner als die direkten Eltern der gefundenen <c>Run_*</c>-Ordner und arbeitet
        /// damit sowohl auf Rider- als auch auf Session-/Track-Ebene. Non-fatal und guarded.
        /// </summary>
        /// <returns>Den ggf. aktualisierten <paramref name="root"/>-Pfad (falls dieser selbst umbenannt wurde).</returns>
        private string NormalizeDeviceFolders(string root)
        {
            try
            {
                // Geräteordner (direkte Eltern von Run_*-Ordnern) rein per System.IO ermitteln –
                // NICHT über KtmDataHandler, dessen native Verzeichnis-Handles die Umbenennung
                // sonst mit "Access denied" blockieren. Tiefste zuerst, damit verschachtelte
                // Umbenennungen die Pfade nicht invalidieren.
                var deviceFolders = FindDeviceFolders(root)
                    .OrderByDescending(d => d.Length)
                    .ToList();

                string rootFull = Path.GetFullPath(root)
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                string result = root;

                foreach (var dir in deviceFolders)
                {
                    string? renamed = TryRenameDeviceFolder(dir);
                    if (renamed is not null
                        && string.Equals(
                            Path.GetFullPath(dir).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                            rootFull, StringComparison.OrdinalIgnoreCase))
                    {
                        result = renamed; // root war selbst der Geräteordner
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[normalize] ⚠️ Geräteordner-Normalisierung übersprungen: {ex.Message}");
                return root;
            }
        }

        /// <summary>
        /// Ermittelt alle Geräteordner unter <paramref name="root"/> rein per System.IO:
        /// Ordner, die direkt einen <c>Run_*</c>-Unterordner enthalten. Steigt nicht in Run-/Lap-Ordner ab,
        /// sodass ein per-Run-Aufruf (KRAKEN) den Elternordner nicht umbenennt.
        /// </summary>
        private static List<string> FindDeviceFolders(string root)
        {
            var found = new List<string>();
            var queue = new Queue<string>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                var dir = queue.Dequeue();
                string[] subDirs;
                try { subDirs = Directory.GetDirectories(dir); }
                catch { continue; }

                if (subDirs.Any(s => Path.GetFileName(s).StartsWith("Run_", StringComparison.OrdinalIgnoreCase)))
                    found.Add(dir); // Geräteordner – nicht weiter absteigen
                else
                    foreach (var s in subDirs) queue.Enqueue(s);
            }
            return found;
        }

        /// <summary>
        /// Benennt einen einzelnen Geräteordner kanonisch um (<c>"AA 01"</c> → <c>"AA01"</c>).
        /// Non-fatal und guarded: nur bei nicht-kanonischem Namen und wenn der Zielordner frei ist.
        /// </summary>
        /// <returns>Den neuen Pfad bei erfolgreicher Umbenennung, sonst <c>null</c>.</returns>
        private static string? TryRenameDeviceFolder(string deviceFolder)
        {
            string name = new DirectoryInfo(deviceFolder).Name;
            if (BikeAlias.IsCanonical(name)) return null;

            string canonical = BikeAlias.Normalize(name);
            if (string.IsNullOrEmpty(canonical) || string.Equals(canonical, name, StringComparison.Ordinal))
                return null;

            string? parent = Path.GetDirectoryName(deviceFolder);
            if (string.IsNullOrEmpty(parent)) return null;
            string target = Path.Combine(parent, canonical);

            if (Directory.Exists(target) || File.Exists(target))
            {
                Console.WriteLine($"[normalize] Zielordner existiert bereits – überspringe Umbenennung \"{name}\" → \"{canonical}\".");
                return null;
            }

            try
            {
                Directory.Move(deviceFolder, target);
                Console.WriteLine($"[normalize] Geräteordner umbenannt: \"{name}\" → \"{canonical}\".");
                return target;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[normalize] ⚠️ Umbenennung fehlgeschlagen für \"{name}\": {ex.Message}. Verarbeitung mit Originalnamen.");
                return null;
            }
        }

        /// <summary>
        /// Ermittelt die Run-Metadaten aus einer HED-Datei oder der KOMPASS-Datenbank.
        /// </summary>
        /// <returns>Die Metadaten (oder <c>null</c>) und ob eine vorhandene KOMPASS.json ungültig war (Exit 7 statt 3).</returns>
        private async Task<(RunMetadata? Metadata, bool KompassInvalid)> ResolveMetadataAsync(string runPath)
        {
            var riderPath = Path.GetFullPath(Path.Combine(runPath, ".."));
            Console.WriteLine($"Rider Path: {riderPath}");

            if (!Directory.Exists(riderPath))
            {
                Console.WriteLine($"Rider Path existiert nicht: {riderPath}");
                return (null, false);
            }

            RunMetadata metadata;
            var hedService = new HedFileService();

            // --- Versuch 1: .HED-Datei suchen ---
            var hedFilePath = hedService.FindAndPrepareHedFile(runPath, riderPath);
            if (hedFilePath != null)
            {
                Console.WriteLine("Metadaten werden aus .HED-Datei geladen.");
                metadata = hedService.ReadMetadata(hedFilePath);
                EnrichPathMetadata(metadata, runPath);
                await CompleteFromKompassAsync(metadata, runPath);
            }
            else
            {
                // --- Versuch 2: KOMPASS-Datenbank ---
                Console.WriteLine("Kein .HED-File im Rider- oder Runordner gefunden.");
                Console.WriteLine("Lade KOMPASS...");

                if (!Directory.Exists(_config.DatabaseDirectory))
                {
                    Console.WriteLine($"Datenbankverzeichnis nicht gefunden: {_config.DatabaseDirectory}");
                    return (null, false);
                }

                var kompass = new KompassService();
                if (!await kompass.LoadAsync(_config.DatabaseFilePath))
                {
                    return (null, kompass.LastLoadInvalid);
                }

                metadata = kompass.BuildMetadata(runPath, _browse, _config.TeamMappings);
                EnrichPathMetadata(metadata, runPath);
                hedService.WriteHedFile(runPath, metadata);
            }

            await EnrichTrackMetadataAsync(metadata);
            return (metadata, false);
        }

        /// <summary>
        /// Ergänzt aus einer .HED gelesene Metadaten um fehlende Felder aus Pfad + KOMPASS (z.B. leerer riderName
        /// oder Team/Saison/Disziplin aus einer älteren .HED). Vorhandene .HED-Werte bleiben unverändert. Non-fatal.
        /// </summary>
        private async Task CompleteFromKompassAsync(RunMetadata metadata, string runPath)
        {
            if (!File.Exists(_config.DatabaseFilePath))
                return;

            var kompass = new KompassService();
            if (!await kompass.LoadAsync(_config.DatabaseFilePath))
                return;

            var derived = new RunMetadata
            {
                EventName = metadata.EventName,
                SessionName = metadata.SessionName,
                BikeName = metadata.BikeName,
                RunNumber = metadata.RunNumber,
            };
            kompass.CompleteFromPathAndKompass(derived, runPath, _config.TeamMappings);

            var differences = MetadataMerger.Merge(metadata, derived, _config.PreferHedMetadata);
            foreach (var d in differences)
            {
                Console.WriteLine(_config.PreferHedMetadata
                    ? $"[metadata] ⚠️ {d.Field}: .HED \"{d.HedValue}\" weicht von KOMPASS \"{d.KompassValue}\" ab – .HED-Wert bleibt (PreferHedMetadata)."
                    : $"[metadata] {d.Field}: .HED \"{d.HedValue}\" → \"{d.KompassValue}\" (KOMPASS/Pfad).");
            }
        }

        /// <summary>
        /// Füllt fehlende, für den .HED-Dateinamen benötigte Pfad-Metadaten
        /// (SeasonName, EventName, SessionName, BikeName) aus dem Run-Pfad.
        /// </summary>
        private void EnrichPathMetadata(RunMetadata metadata, string runPath)
        {
            if (string.IsNullOrEmpty(metadata.EventName))
                metadata.EventName = _browse.GetEventName(runPath) ?? "";
            if (string.IsNullOrEmpty(metadata.SessionName))
                metadata.SessionName = _browse.GetSessionName(runPath) ?? "";
            if (string.IsNullOrEmpty(metadata.BikeName))
                metadata.BikeName = BikeAlias.Normalize(_browse.GetDeviceName(runPath) ?? "");
            if (string.IsNullOrEmpty(metadata.SeasonName))
                metadata.SeasonName = PathMetadataParser.Parse(runPath, metadata.EventName, _config.TeamMappings, []).SeasonName;
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
