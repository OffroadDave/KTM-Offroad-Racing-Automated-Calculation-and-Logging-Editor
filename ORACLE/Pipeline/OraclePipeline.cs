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

            // .HED in jedem Run-Ordner sicherstellen (kanonischer Name, normalisierter Inhalt).
            new HedFileService().EnsureHedInAllRuns(validRuns, Path.GetDirectoryName(runPath)!, metadata);

            // Optional: Runs per GPS-Ziellinie in Laps aufteilen (Flag --gps-split) vor den Berechnungen.
            if (gpsSplit)
                await RunGpsLapSplitAsync(validRuns, metadata, status);

            // --- Berechnungen (sVehicle + per-Lap-Distanzen) und XML-Properties je Run ---
            var xmlEditor = new XmlRunEditor();
            var importCalculator = new ImportDataCalculator();
            int calcSuccess = 0;
            foreach (var run in validRuns)
            {
                try
                {
                    var calc = importCalculator.RunCalculationPipeline(run);
                    xmlEditor.SetRunProperties(run, metadata.BikeName, metadata.TrackName,
                        metadata.TrackLength, metadata.RiderName, lapDistances: calc.LapDistancesMeters);
                    calcSuccess++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[process] Fehler bei {Path.GetFileName(run)}: {ex.Message}");
                }
            }
            status.SetStep("properties", StepStatus.Success);
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
        /// Erkennt, welche Runs eines Ordners tatsächlich Rennen (Renntempo) sind — im Gegensatz
        /// zu Motor-Blips, Standgas, langsamem Rollen, Sichtungsrunden oder Auslauf.
        /// Read-only: verändert keine Daten. Delegiert an <see cref="RaceSegmentDetector"/>.
        /// </summary>
        /// <param name="folderPath">Rider-/Session-Ordner oder direkter Run-Ordner.</param>
        public OracleExitCode DetectRaces(string folderPath)
        {
            Console.WriteLine($"[detect] Path: {folderPath}");

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"Der angegebene Pfad existiert nicht: {folderPath}");
                return OracleExitCode.DataNotFound;
            }

            var detector = new RaceSegmentDetector(_browse, _config.RaceDetection);
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

            var runs = (_browse.FindRuns(riderPath) ?? Enumerable.Empty<string>())
                .OrderBy(p => Path.GetFileName(p), StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (runs.Count == 0)
            {
                Console.WriteLine($"Keine gültigen Runs gefunden in: {riderPath}");
                var s = OracleStatus.Create(riderPath);
                s.SetStep("discovery", StepStatus.Failed);
                s.ExitCode = (int)OracleExitCode.DataNotFound;
                s.Save(riderPath);
                return OracleExitCode.DataNotFound;
            }

            // Status/Idempotenz gehören in den Geräteordner (Elternordner der Runs, z.B. "…\AA01"),
            // nicht in einen ggf. übergeordneten Session-Ordner (z.B. "…\R1").
            var statusFolder = DetermineStatusFolder(riderPath, runs);

            if (!force && OracleStatus.IsAlreadyProcessed(statusFolder))
            {
                Console.WriteLine("[process-rider] Bereits verarbeitet (oracle.status.json). Überspringe. (--force zum Erzwingen)");
                return OracleExitCode.Success;
            }

            var status = OracleStatus.Create(statusFolder);

            // Metadaten einmalig über einen repräsentativen Run ermitteln.
            var metadata = await ResolveMetadataAsync(runs[0]);
            if (metadata == null)
            {
                Console.WriteLine("Metadaten konnten nicht ermittelt werden.");
                status.SetStep("metadata", StepStatus.Failed);
                status.ExitCode = (int)OracleExitCode.DataNotFound;
                status.Save(statusFolder);
                return OracleExitCode.DataNotFound;
            }
            status.SetStep("metadata", StepStatus.Success, new Dictionary<string, object>
            {
                ["bikeName"] = metadata.BikeName,
                ["riderName"] = metadata.RiderName,
            });

            // .HED in jedem Run-Ordner sicherstellen (kanonischer Name, normalisierter Inhalt).
            new HedFileService().EnsureHedInAllRuns(runs, statusFolder, metadata);

            // Optional: Runs per GPS-Ziellinie in Laps aufteilen (Flag --gps-split), vor der Stint-Verarbeitung.
            if (gpsSplit)
                await RunGpsLapSplitAsync(runs, metadata, status);

            double trackLength = ParseTrackLength(metadata.TrackLength);

            var stintProcessor = new RaceStintProcessor(
                new RaceSegmentDetector(_browse, _config.RaceDetection));
            var result = stintProcessor.ProcessRider(riderPath, metadata, trackLength);

            status.SetStep("raceDetection", StepStatus.Success, new Dictionary<string, object>
            {
                ["raceRuns"] = result.RaceRunCount,
                ["nonRaceRuns"] = result.NonRaceRunCount,
                ["totalRuns"] = runs.Count,
            });
            status.SetStep("stint", result.RaceRunCount > 0 ? StepStatus.Success : StepStatus.Partial,
                new Dictionary<string, object>
                {
                    ["totalRaceLaps"] = result.TotalRaceLaps,
                    ["totalRaceDistanceMeters"] = Math.Round(result.TotalRaceDistanceMeters, 1),
                });

            status.ExitCode = (int)OracleExitCode.Success;
            status.Save(statusFolder);

            Console.WriteLine(
                $"[process-rider] Abgeschlossen: {result.RaceRunCount} Renn-Run(s), " +
                $"{result.TotalRaceLaps} Renn-Laps, {result.TotalRaceDistanceMeters:F0} m Gesamtdistanz.");
            return OracleExitCode.Success;
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

        /// <summary>Parst die Track-Länge (String aus Metadaten) zu Metern; 0 wenn unbekannt.</summary>
        private static double ParseTrackLength(string trackLength)
            => double.TryParse(trackLength, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double m) ? m : 0.0;

        /// <summary>
        /// Teilt die angegebenen Runs per GPS-Ziellinie in Laps: ermittelt die Ziellinie aus KOMPASS
        /// (Track-Koordinate) und schreibt die Lap-Grenzen headless über <see cref="GpsLapSplitService"/>.
        /// Non-fatal: ohne Ziellinie oder GPS wird übersprungen, ein Fehler stoppt die Pipeline nicht.
        /// </summary>
        private async Task RunGpsLapSplitAsync(IReadOnlyList<string> runs, RunMetadata metadata, OracleStatus status)
        {
            var finishLine = await ResolveFinishLineAsync(metadata);
            if (finishLine is not { } line)
            {
                Console.WriteLine($"[gps-split] Übersprungen: keine Ziellinie in KOMPASS für Track \"{metadata.TrackName}\" (Lat/Lon leer?).");
                status.SetStep("gpsSplit", StepStatus.Skipped, new Dictionary<string, object> { ["reason"] = "no finish line" });
                return;
            }

            var service = new GpsLapSplitService();
            var options = _config.GpsLapSplit.ToOptions();
            int split = 0, skipped = 0, failed = 0;
            foreach (var run in runs)
            {
                var outcome = service.SplitRun(run, line, options);
                Console.WriteLine($"[gps-split] {Path.GetFileName(run)}: {outcome.Message}");
                switch (outcome.Status)
                {
                    case GpsLapSplitOutcomeStatus.Split: split++; break;
                    case GpsLapSplitOutcomeStatus.Failed: failed++; break;
                    default: skipped++; break;
                }
            }

            Console.WriteLine($"[gps-split] Abgeschlossen: {split} gesplittet, {skipped} übersprungen, {failed} Fehler.");
            status.SetStep("gpsSplit", failed == 0 ? StepStatus.Success : StepStatus.Partial, new Dictionary<string, object>
            {
                ["split"] = split,
                ["skipped"] = skipped,
                ["failed"] = failed,
            });
        }

        /// <summary>
        /// Ermittelt die Ziellinien-Koordinate für den Track der Metadaten aus KOMPASS.
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

            return kompass.FindFinishLineByTrack(metadata.TrackName);
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
        private async Task<RunMetadata?> ResolveMetadataAsync(string runPath)
        {
            var riderPath = Path.GetFullPath(Path.Combine(runPath, ".."));
            Console.WriteLine($"Rider Path: {riderPath}");

            if (!Directory.Exists(riderPath))
            {
                Console.WriteLine($"Rider Path existiert nicht: {riderPath}");
                return null;
            }

            RunMetadata metadata;
            var hedService = new HedFileService();

            // --- Versuch 1: .HED-Datei suchen ---
            var hedFilePath = hedService.FindAndPrepareHedFile(runPath, riderPath);
            if (hedFilePath != null)
            {
                Console.WriteLine("Metadaten werden aus .HED-Datei geladen.");
                metadata = hedService.ReadMetadata(hedFilePath);
            }
            else
            {
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
                    return null;

                metadata = kompass.BuildMetadata(runPath, _browse, _config.TeamMappings);
                hedService.WriteHedFile(runPath, metadata);
            }

            // Für den kanonischen .HED-Dateinamen benötigte Pfad-Felder sicherstellen
            // (die minimale .HED enthält Season/Event/Session nicht).
            EnrichPathMetadata(metadata, runPath);
            await EnrichTrackMetadataAsync(metadata);
            return metadata;
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
                metadata.SeasonName = ExtractSeason(runPath);
        }

        /// <summary>Extrahiert die Saison (4-stelliges Jahr, z.B. "2026") als Pfadsegment.</summary>
        private static string ExtractSeason(string runPath)
            => runPath.Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
                    StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(p => p.Length == 4 && p.StartsWith("20", StringComparison.Ordinal) && p.All(char.IsDigit)) ?? "";

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
