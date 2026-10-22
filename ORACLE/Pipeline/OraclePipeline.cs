using KtmDataHandler.Marelli.Wintax;
using KtmDataHandler.Models.Channels;
using Ktm.Racing.Domain.Geo;

namespace Ktm.Racing.Oracle
{
    /// <summary>
    /// Central orchestration class for ORACLE processing steps.
    /// 
    /// <para><b>Commands:</b></para>
    /// <list type="bullet">
    ///   <item><see cref="ProcessPathAsync"/> — postprocessing of a run or rider/session folder</item>
    ///   <item><see cref="ValidateRun"/> — data integrity check of a run</item>
    ///   <item><see cref="DetectRaces"/>, <see cref="DetectStarts"/> — race and start detection (read-only)</item>
    ///   <item><see cref="SplitLapsAsync"/> — split runs into laps at the GPS finish line</item>
    /// </list>
    /// 
    /// <para>Called from <c>Program.Main()</c> via command routing.
    /// Each method can be used standalone (for KRAKEN as separate EXE jobs
    /// or for future library use).</para>
    /// 
    /// <para><b>Dependencies:</b> via constructor injection for testability.
    /// Use <see cref="Create()"/> for standard instantiation or the constructor
    /// for unit tests with mocked dependencies.</para>
    /// </summary>
    public class OraclePipeline
    {
        private readonly IMarelliDataBrowse _browse;
        private readonly OracleConfig _config;

        // Runs repaired in this invocation (for status/log).
        private readonly List<RunRepairResult> _repairs = [];

        /// <summary>
        /// Creates a new OraclePipeline with explicit dependencies.
        /// Primarily for unit tests with mocked services.
        /// </summary>
        /// <param name="browse">IMarelliDataBrowse for run discovery and path validation</param>
        /// <param name="config">ORACLE configuration (paths, TeamMappings)</param>
        public OraclePipeline(IMarelliDataBrowse browse, OracleConfig config)
        {
            ArgumentNullException.ThrowIfNull(browse);
            ArgumentNullException.ThrowIfNull(config);
            _browse = browse;
            _config = config;
        }

        /// <summary>
        /// Factory method: creates an OraclePipeline with standard dependencies.
        /// Loads the configuration from <c>oracle-config.json</c> and creates a
        /// new <see cref="MarelliDataBrowse"/> instance.
        /// </summary>
        public static OraclePipeline Create()
        {
            return new OraclePipeline(new MarelliDataBrowse(), OracleConfig.Load());
        }
        /// <summary>
        /// Complete postprocessing pipeline for a single run:
        /// metadata → properties → calculations.
        /// 
        /// <para><b>Steps:</b></para>
        /// <list type="number">
        ///   <item>Idempotency check (oracle.status.json)</item>
        ///   <item>Validate run folder (IsRunLocation)</item>
        ///   <item>Load configuration (oracle-config.json)</item>
        ///   <item>Determine metadata (HED file or KOMPASS fallback)</item>
        ///   <item>Discover valid runs</item>
        ///   <item>Set run/lap properties</item>
        ///   <item>Run calculation pipeline (sVehicle etc.)</item>
        ///   <item>Write status file</item>
        /// </list>
        /// </summary>
        /// <param name="runPath">Path to the Run_xxx folder</param>
        /// <param name="force">True to skip the idempotency check</param>
        /// <returns>Exit code for KRAKEN</returns>
        public async Task<OracleExitCode> ProcessRunAsync(string runPath, bool force = false, bool gpsSplit = false)
        {
            Console.WriteLine($"[process] Run Path: {runPath}");

            if (!Directory.Exists(runPath))
            {
                Console.WriteLine($"The specified run path does not exist: {runPath}");
                return OracleExitCode.DataNotFound;
            }

            // Roll back an interrupted GPS lap split first (under device and run lock).
            if (RecoverInterruptedSplits(runPath) is { } recoveryExit)
                return recoveryExit;

            var devicePath = Path.GetDirectoryName(Path.GetFullPath(runPath))!;
            using var deviceLock = ProcessingLock.TryAcquire(devicePath);
            if (deviceLock is null)
            {
                Console.WriteLine($"[process] {devicePath} is currently being processed by another ORACLE process – try again later.");
                return OracleExitCode.ResourceLocked;
            }

            // Parallel ORACLE jobs must not process the same run at the same time.
            using var processingLock = ProcessingLock.TryAcquire(runPath);
            if (processingLock is null)
            {
                Console.WriteLine($"[process] {runPath} is currently being processed by another ORACLE process – try again later.");
                return OracleExitCode.ResourceLocked;
            }

            // --- Idempotency check ---
            if (!force && OracleStatus.IsAlreadyProcessed(runPath))
            {
                var lengthCheck = CheckProcessedTrackLength([runPath], runPath);
                if (lengthCheck is { } code)
                {
                    Console.WriteLine("[process] Run already processed successfully (oracle.status.json). Skipping.");
                    Console.WriteLine("[process] Use --force to process again.");
                    // The weather does not depend on the track length: catch it up even while the length is pending.
                    await CatchUpWeatherAsync([runPath], runPath);
                    return code;
                }
            }

            var status = OracleStatus.Create(runPath);

            if (!_browse.IsRunLocation(runPath))
            {
                Console.WriteLine($"Warning: the path is not a valid WinTAX4 run: {runPath}");
            }

            // --- Determine metadata ---
            RecordInputs(status);
            var (metadata, kompassInvalid) = await ResolveMetadataAsync(runPath);
            if (metadata == null)
            {
                var code = kompassInvalid ? OracleExitCode.InvalidData : OracleExitCode.DataNotFound;
                Console.WriteLine("Metadata could not be determined.");
                status.SetStep("metadata", StepStatus.Failed);
                status.ExitCode = (int)code;
                status.Save(runPath);
                return code;
            }
            SetMetadataStep(status, metadata);

            // --- Find valid runs ---
            var validRuns = DiscoverRuns(runPath);
            if (validRuns.Count == 0)
            {
                Console.WriteLine($"No valid runs found in: {runPath}");
                status.SetStep("discovery", StepStatus.Failed);
                status.ExitCode = (int)OracleExitCode.DataNotFound;
                status.Save(runPath);
                return OracleExitCode.DataNotFound;
            }

            Console.WriteLine($"{validRuns.Count} valid run(s) found.");

            // Determine the track length up front from KOMPASS/CIRCUITS/.HED/suggestions (MotoDB compares exactly).
            var lengthGate = TrackLengthGate.FromConfig(_config);
            var knownLength = ResolveKnownTrackLength(lengthGate, metadata);

            // Ensure a .HED in every run folder (canonical name, normalized content).
            new HedFileService().EnsureHedInAllRuns(validRuns, Path.GetDirectoryName(runPath)!, metadata);

            // Optional: split runs into laps at the GPS finish line (flag --gps-split) before the calculations.
            if (gpsSplit)
                await RunGpsLapSplitAsync(validRuns, metadata, status);

            // --- Calculations (sVehicle + per-lap distances) and XML properties per run ---
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
                    Console.WriteLine($"[process] Error in {Path.GetFileName(run)}: {ex.Message}");
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

            // Without a single successful calculation the run is not processed: no "Done", no MotoDB start.
            if (calcSuccess == 0)
            {
                Console.WriteLine("[process] Calculations did not succeed for any run – status remains open (retry on next invocation).");
                status.ExitCode = (int)OracleExitCode.GeneralError;
                status.Save(runPath);
                return OracleExitCode.GeneralError;
            }

            // --- Weather during the run (non-fatal, never changes the exit code) ---
            await ApplyWeatherAsync(validRuns, metadata, status);

            // --- Check/estimate track length: MotoDB must not run without an unambiguous length ---
            var lengthExit = FinishTrackLength(lengthGate, knownLength, metadata, validRuns,
                Path.GetDirectoryName(runPath)!, lapDistances, runPath, status);
            if (lengthExit != OracleExitCode.Success)
            {
                status.ExitCode = (int)lengthExit;
                status.Save(runPath);
                return lengthExit;
            }

            // --- Write status file ---
            var processExit = calcSuccess == validRuns.Count ? OracleExitCode.Success : OracleExitCode.PartialSuccess;
            status.ExitCode = (int)processExit;
            status.Save(runPath);

            Console.WriteLine("[process] Completed.");
            return processExit;
        }

        /// <summary>
        /// Validates the data integrity of a run:
        /// checks file structure, loadability and available channels.
        /// 
        /// <para><b>Checks:</b></para>
        /// <list type="bullet">
        ///   <item>Run folder exists and is a valid WinTAX4 run</item>
        ///   <item>At least one Lap_* folder with cableData.ztx present</item>
        ///   <item>cableData.ztx files have a plausible size (> 0 bytes)</item>
        ///   <item>Data can be loaded via KtmDataHandler</item>
        ///   <item>At least one channel is present</item>
        /// </list>
        /// </summary>
        /// <param name="runPath">Path to the Run_xxx folder</param>
        /// <returns>Exit code: Success=OK, DataNotFound=error</returns>
        public OracleExitCode ValidateRun(string runPath)
        {
            Console.WriteLine($"[validate] Run Path: {runPath}");

            // --- Existence check ---
            if (!Directory.Exists(runPath))
            {
                Console.WriteLine($"Run folder does not exist: {runPath}");
                return OracleExitCode.DataNotFound;
            }

            // --- WinTAX4 run validation ---
            if (!_browse.IsRunLocation(runPath))
            {
                Console.WriteLine($"Not a valid WinTAX4 run: {runPath}");
                return OracleExitCode.DataNotFound;
            }

            // --- Check lap folders ---
            var lapDirs = Directory.GetDirectories(runPath, "Lap_*", SearchOption.TopDirectoryOnly);
            if (lapDirs.Length == 0)
            {
                Console.WriteLine("No Lap_* folders found.");
                return OracleExitCode.DataNotFound;
            }
            Console.WriteLine($"  {lapDirs.Length} lap folder(s) found.");

            // --- Check cableData.ztx files ---
            int validLaps = 0;
            int emptyFiles = 0;
            foreach (var lapDir in lapDirs)
            {
                var ztxFile = Path.Combine(lapDir, "cableData.ztx");
                if (!File.Exists(ztxFile))
                {
                    Console.WriteLine($"  ⚠️ cableData.ztx missing in: {Path.GetFileName(lapDir)}");
                    continue;
                }

                var fileInfo = new FileInfo(ztxFile);
                if (fileInfo.Length == 0)
                {
                    Console.WriteLine($"  ⚠️ cableData.ztx is empty (0 bytes): {Path.GetFileName(lapDir)}");
                    emptyFiles++;
                    continue;
                }

                validLaps++;
            }

            if (validLaps == 0)
            {
                Console.WriteLine("No valid cableData.ztx files found.");
                return OracleExitCode.DataNotFound;
            }

            Console.WriteLine($"  {validLaps} valid cableData.ztx file(s), {emptyFiles} empty.");

            // --- Check data is loadable (not loadable = corrupt → InvalidData, no retry) ---
            var ztxRead = KtmDataHandlerFactory.CreateReader();
            try
            {
                ztxRead.SetDataLocation([runPath]);
                var load = ztxRead.LoadRun();
                if (load <= 0)
                {
                    Console.WriteLine($"  ❌ Data could not be loaded (LoadRun {load}).");
                    return OracleExitCode.InvalidData;
                }

                var channelNames = ztxRead.GetChannelsNames();
                if (channelNames == null || channelNames.Length == 0)
                {
                    Console.WriteLine("  ⚠️ No channels found in the data.");
                    return OracleExitCode.InvalidData;
                }

                Console.WriteLine($"  {channelNames.Length} channel(s) available.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ Data could not be loaded: {ex.Message}");
                return OracleExitCode.InvalidData;
            }
            finally
            {
                ztxRead.ReleaseObjects();
            }

            Console.WriteLine("[validate] All checks passed.");
            return OracleExitCode.Success;
        }

        /// <summary>
        /// Detects which runs of a folder are actually races (race pace) — as opposed to
        /// engine blips, idling, slow rolling, sighting laps or cool-down.
        /// Read-only: does not modify any data. Delegates to <see cref="RaceSegmentDetector"/>.
        /// </summary>
        /// <param name="folderPath">Rider/session folder or direct run folder.</param>
        /// <summary>Detects valid starts (gate start) in all runs of a path (read-only, see <see cref="StartDetector"/>).</summary>
        public OracleExitCode DetectStarts(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"The specified path does not exist: {path}");
                return OracleExitCode.DataNotFound;
            }
            return new StartAnalyzer(_browse).Run(path);
        }

        public OracleExitCode DetectRaces(string folderPath)
        {
            Console.WriteLine($"[detect] Path: {folderPath}");

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"The specified path does not exist: {folderPath}");
                return OracleExitCode.DataNotFound;
            }

            var detector = new RaceSegmentDetector(_browse, _config.RaceDetection, _config.Gps);
            var result = detector.DetectRaces(folderPath);

            if (result.Runs.Count == 0)
            {
                Console.WriteLine("[detect] No runs found.");
                return OracleExitCode.DataNotFound;
            }

            foreach (var f in result.Runs)
            {
                string verdict = f.Verdict switch
                {
                    RaceRunVerdict.Race => "RACE",
                    RaceRunVerdict.Error => "ERROR",
                    _ => "-",
                };
                Console.WriteLine(
                    $"  {f.RunFolderName,-12} dist={f.GpsDistanceMeters,7:F0} m  vGPS={f.MeanGpsSpeedKph,5:F1} kph  " +
                    $"Throttle={f.MeanThrottlePercent,5:F1} %  rpm={f.MeanEngineRpm,6:F0}  " +
                    $"=> {verdict}{(f.LowConfidence ? " (review)" : "")}  [{f.Reason}]");
            }

            var raceNames = result.RaceRunPaths.Select(Path.GetFileName);
            Console.WriteLine($"[detect] Detected as race: {(result.RaceCount > 0 ? string.Join(", ", raceNames) : "(none)")}");
            return OracleExitCode.Success;
        }

        /// <summary>
        /// Routes a <c>process</c> call: single run → per-run pipeline (KRAKEN-compatible),
        /// rider/session folder → race-aware, continuous processing (<see cref="ProcessRiderAsync"/>).
        /// </summary>
        public async Task<OracleExitCode> ProcessPathAsync(string path, bool force = false, bool gpsSplit = false)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"The specified path does not exist: {path}");
                return OracleExitCode.DataNotFound;
            }

            // Normalize device folder names BEFORE KtmDataHandler touches the path:
            // its native directory handles (IsRunLocation/FindRuns) would otherwise block a later
            // Directory.Move with "Access denied".
            if (_config.NormalizeDeviceFolderName)
                path = NormalizeDeviceFolders(path);

            // Roll back interrupted GPS lap splits first (under lock) – before repair, detection and idempotency.
            if (RecoverInterruptedSplits(path) is { } recoveryExit)
                return recoveryExit;

            // For the same reason before the WinTAX API: clear orphaned lap folders (split/append leftovers without data)
            // out of the runs so that WinTAX recognizes the runs again and they get processed.
            if (_config.RepairOrphanLapFolders)
                _repairs.AddRange(RunRepair.RepairAll(path));

            return _browse.IsRunLocation(path)
                ? await ProcessRunAsync(path, force, gpsSplit)
                : await ProcessRiderAsync(path, force, gpsSplit);
        }

        /// <summary>
        /// Processes an entire rider/session folder in a race-aware way (option C):
        /// the runs detected as races get continuous <c>sVehicle</c> distance and
        /// consecutive lap numbering; non-race runs are processed independently.
        /// The original folder structure remains unchanged.
        /// </summary>
        public async Task<OracleExitCode> ProcessRiderAsync(string riderPath, bool force = false, bool gpsSplit = false)
        {
            Console.WriteLine($"[process-rider] Rider Path: {riderPath}");

            // Rename device folders with spaces canonically ("AA 01" → "AA01") so that the
            // bikeName is consistently without spaces in folders and XML. Non-fatal + guarded.
            if (_config.NormalizeDeviceFolderName)
                riderPath = NormalizeDeviceFolders(riderPath);

            if (RecoverInterruptedSplits(riderPath) is { } recoveryExit)
                return recoveryExit;

            var runs = (_browse.FindRuns(riderPath) ?? Enumerable.Empty<string>())
                .OrderBy(p => Path.GetFileName(p), StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Never silently skip run folders that the WinTAX API does not recognize (corrupt).
            var skipped = RunFolderInspector.FindSkipped(riderPath, runs);

            // Multiple device folders (e.g. session folder "BC" with AA01 + AA02): process each device folder independently
            // – with its own metadata (bike/rider), its own .HED and its own status.
            var devices = runs.Concat(skipped.Select(s => s.RunPath))
                .Select(Path.GetDirectoryName)
                .Where(d => !string.IsNullOrEmpty(d))
                .Select(d => d!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(d => d, StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (devices.Count > 1)
            {
                Console.WriteLine($"[process-rider] {devices.Count} device folders found – each is processed separately: " +
                                  string.Join(", ", devices.Select(Path.GetFileName)));
                var codes = new List<OracleExitCode>();
                foreach (var device in devices)
                {
                    var code = await ProcessRiderAsync(device, force, gpsSplit);
                    Console.WriteLine($"[process-rider] {Path.GetFileName(device)}: exit code {(int)code} ({code})");
                    codes.Add(code);
                }
                return OracleExitCodes.Combine(codes);
            }

            var corrupt = skipped.Where(s => !s.IsEmpty).ToList();
            var empty = skipped.Where(s => s.IsEmpty).ToList();
            foreach (var s in corrupt)
                Console.WriteLine($"[discovery] ⚠️ Run skipped (not a valid WinTAX4 run): {s.RunPath} – {s.Reason}");
            foreach (var s in empty)
                Console.WriteLine($"[discovery] Empty run (no lap data recorded): {Path.GetFileName(s.RunPath)} – only the metadata is set.");
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
                    // Only empty runs: no measurement data, not an error.
                    Console.WriteLine($"[process-rider] {riderPath}: only empty runs (no lap data) – nothing to calculate.");
                    s.SetStep("discovery", StepStatus.Skipped, new Dictionary<string, object> { ["emptyRuns"] = emptyRuns.Select(Path.GetFileName).ToList() });
                    s.ExitCode = (int)OracleExitCode.Success;
                    s.Save(riderPath);
                    return OracleExitCode.Success;
                }

                var code = corrupt.Count > 0 ? OracleExitCode.InvalidData : OracleExitCode.DataNotFound;
                Console.WriteLine(corrupt.Count > 0
                    ? $"No processable runs in: {riderPath} – {corrupt.Count} corrupt run folder(s) (see above). Repair in WinTAX, then run again with --force."
                    : $"No valid runs found in: {riderPath}");
                s.SetStep("discovery", StepStatus.Failed, corrupt.Count > 0
                    ? new Dictionary<string, object> { ["skippedRuns"] = skippedDetails }
                    : null);
                s.ExitCode = (int)code;
                s.Save(riderPath);
                return code;
            }

            // Status/idempotency belong in the device folder (parent folder of the runs, e.g. "…\AA01"),
            // not in a possibly higher-level session folder (e.g. "…\R1").
            var statusFolder = DetermineStatusFolder(riderPath, runs);

            // Parallel ORACLE jobs (KRAKEN) must not write to the same device folder at the same time.
            using var processingLock = ProcessingLock.TryAcquire(statusFolder);
            if (processingLock is null)
            {
                Console.WriteLine($"[process-rider] {statusFolder} is currently being processed by another ORACLE process – try again later.");
                return OracleExitCode.ResourceLocked;
            }

            if (!force && OracleStatus.IsAlreadyProcessed(statusFolder))
            {
                var lengthCheck = CheckProcessedTrackLength(runs, statusFolder);
                if (lengthCheck is { } code)
                {
                    Console.WriteLine("[process-rider] Already processed (oracle.status.json). Skipping. (--force to override)");
                    // The weather does not depend on the track length: catch it up even while the length is pending.
                    await CatchUpWeatherAsync(runs, statusFolder);
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

            // Determine metadata once via a representative run (all runs belong to exactly one device).
            var (metadata, kompassInvalid) = await ResolveMetadataAsync(runs[0]);
            if (metadata == null)
            {
                var code = kompassInvalid ? OracleExitCode.InvalidData : OracleExitCode.DataNotFound;
                Console.WriteLine("Metadata could not be determined.");
                status.SetStep("metadata", StepStatus.Failed);
                status.ExitCode = (int)code;
                status.Save(statusFolder);
                return code;
            }
            SetMetadataStep(status, metadata);

            // Determine the track length up front (also for the relative distance threshold of race detection).
            var lengthGate = TrackLengthGate.FromConfig(_config);
            var knownLength = ResolveKnownTrackLength(lengthGate, metadata);

            // Ensure a .HED in every run folder (canonical name, normalized content) – also in empty runs.
            var allRuns = runs.Concat(emptyRuns).ToList();
            new HedFileService().EnsureHedInAllRuns(allRuns, statusFolder, metadata);
            SetEmptyRunProperties(emptyRuns, metadata);

            // Optional: split runs into laps at the GPS finish line (flag --gps-split), before stint processing.
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

            // Without a single successful calculation the rider is not processed: no "Done"/"LengthPending", no MotoDB start.
            if (result.ProcessedRunPaths.Count == 0 && result.FailedRuns.Count > 0)
            {
                Console.WriteLine("[process-rider] Calculations did not succeed for any run – status remains open (retry on next invocation).");
                status.ExitCode = (int)OracleExitCode.GeneralError;
                status.Save(statusFolder);
                return OracleExitCode.GeneralError;
            }

            // --- Weather during the session (non-fatal, never changes the exit code) ---
            await ApplyWeatherAsync(runs, metadata, status);

            // --- Check/estimate track length: MotoDB must not run without an unambiguous length ---
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
                $"[process-rider] Completed: {result.RaceRunCount} race run(s), " +
                $"{result.TotalRaceLaps} race laps, {result.TotalRaceDistanceMeters:F0} m total distance." +
                (repairedDetails.Count > 0 ? $" {repairedDetails.Count} run(s) repaired." : string.Empty) +
                (empty.Count > 0 ? $" {empty.Count} empty run(s) (metadata only)." : string.Empty) +
                (corrupt.Count > 0 ? $" {corrupt.Count} corrupt run folder(s) NOT processed." : string.Empty) +
                (result.FailedRuns.Count > 0 ? $" {result.FailedRuns.Count} run(s) failed." : string.Empty));
            return exit;
        }

        /// <summary>Sets the run metadata (Car.xml, Session.xml) in empty runs (no lap data) so that they are named consistently.</summary>
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
                    Console.WriteLine($"[process-rider] ⚠️ {Path.GetFileName(run)}: metadata could not be written – {ex.Message}");
                }
            }
        }

        /// <summary>Writes the metadata step; without a resolved rider (KOMPASS) it is only partially successful.</summary>
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
                Console.WriteLine("[metadata] ⚠️ Metadata incomplete (rider or track not resolved) – maintain in KODE.");
            status.SetStep("metadata", complete ? StepStatus.Success : StepStatus.Partial, details);
        }

        /// <summary>Records the state of the inputs (KOMPASS, loadChannels) in log and status.</summary>
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
        /// Determines the folder for <c>oracle.status.json</c>: the device folder (common
        /// parent folder of the runs, e.g. "…\AA01"). Only if the runs are spread across multiple device
        /// folders does it fall back to <paramref name="riderPath"/>.
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
        /// Parses the track length (string from metadata) to meters like the track length check (whole meters, e.g. "1592");
        /// 0 if unknown or invalid ("1850,5" or "1.850" are not read as 18505 or 1.85 m respectively).
        /// </summary>
        internal static double ParseTrackLength(string trackLength)
            => Ktm.Racing.Domain.Kompass.TrackLengthRules.Parse(trackLength) ?? 0.0;

        /// <summary>
        /// Determines the track length before the calculations from KOMPASS, CIRCUITS, .HED or open suggestions
        /// and adopts it into the metadata (for .HED, Session.xml and race detection).
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
        /// Completes the track length check (estimating from the lap distances if necessary), writes a changed
        /// length to .HED and Session.xml of all runs and returns the exit code: <see cref="OracleExitCode.Success"/>,
        /// <see cref="OracleExitCode.ResourceLocked"/> (loadChannels.xlsx locked) or
        /// <see cref="OracleExitCode.TrackLengthMissing"/> (no unambiguous length, MotoDB must not run).
        /// </summary>
        private OracleExitCode FinishTrackLength(TrackLengthGate gate, TrackLengthDecision known, RunMetadata metadata,
            IReadOnlyList<string> runs, string hedFolder, IEnumerable<IReadOnlyList<double>> lapDistances,
            string origin, OracleStatus status)
        {
            // If the own laps are not sufficient, the laps of already processed runs of the same event are used.
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
                    Console.WriteLine("[trackLength] No unambiguous track length – MotoDB will not be started. Enter the length in KODE and run again.");
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
        /// For already processed runs (idempotency skip) checks the track length: if it is known – or can be estimated
        /// from the already written lap distances (own runs + event) – .HED, Session.xml and CIRCUITS are updated on
        /// deviation and the status is updated (also after exit 5/6: no full processing needed).
        /// <c>null</c> = no length and no stored lap distances → process completely again.
        /// </summary>
        private OracleExitCode? CheckProcessedTrackLength(IReadOnlyList<string> runs, string origin)
        {
            var hedFile = runs
                .Where(Directory.Exists)
                .Select(r => Directory.EnumerateFiles(r, "*.HED", SearchOption.TopDirectoryOnly).FirstOrDefault())
                .FirstOrDefault(f => f is not null);
            if (hedFile is null)
            {
                Console.WriteLine("[trackLength] Already processed, but no .HED found – processing again.");
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
                Console.WriteLine($"[trackLength] {known.Message} Already processed, but without length and without lap distances – processing again.");
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
                Console.WriteLine($"[trackLength] Length changed: {(metadata.TrackLength.Length == 0 ? "(empty)" : metadata.TrackLength)} → {decision.LengthText} m ({decision.Source}).");
                var xml = new XmlRunEditor();
                foreach (var run in runs)
                {
                    hed.SetTrackLength(run, decision.LengthText);
                    xml.SetTrackLength(run, decision.LengthText);
                }
            }

            // Record the correction / catch-up length traceably in the status.
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
                    Console.WriteLine("[trackLength] Length caught up – status set to successful (no full processing needed).");
                }
                previous.Save(origin);
            }

            // Update the surface from KOMPASS if it has changed.
            var surface = gate.KompassSurface(metadata.TrackName);
            if (surface.Length > 0 && !string.Equals(surface, metadata.Surface, StringComparison.Ordinal))
            {
                Console.WriteLine($"[hed] Surface changed: \"{metadata.Surface}\" → \"{surface}\".");
                foreach (var run in runs)
                    hed.SetTrackSurface(run, surface);
            }

            return OracleExitCode.Success;
        }

        /// <summary>
        /// Splits the given runs into laps at the GPS finish line (<see cref="GpsLapSplitService"/>): one finish line for all runs
        /// of the track from the KOMPASS coordinate, detection of already split runs, writing with backup and rollback.
        /// Non-fatal: skipped without a finish line or GPS, an error does not stop the pipeline.
        /// </summary>
        private async Task<IReadOnlyList<GpsLapSplitOutcome>> RunGpsLapSplitAsync(IReadOnlyList<string> runs, RunMetadata metadata,
            OracleStatus? status, bool dryRun = false, bool lockRuns = false)
        {
            var reference = await ResolveFinishLineAsync(metadata);
            if (reference is not { } point)
            {
                Console.WriteLine($"[gps-split] Skipped: no coordinate in KOMPASS for track \"{metadata.TrackName}\" – enter lat/lon in KODE.");
                status?.SetStep("gpsSplit", StepStatus.Skipped, new Dictionary<string, object> { ["reason"] = "no track coordinate in KOMPASS" });
                return [];
            }

            // Invalidate the status before each lap swap (under the run lock): changed laps (lap numbers, LapDistance,
            // sVehicle) must be recalculated, even if ORACLE aborts right after the swap.
            var service = new GpsLapSplitService(_config.GpsLapSplit, lockRuns: lockRuns, gps: _config.Gps,
                beforeSwap: run => InvalidateProcessedStatus(run, "laps split by GPS finish line"));
            var (line, outcomes) = service.SplitRuns(runs, point, dryRun);
            Console.WriteLine(line.Line is { } l
                ? $"[gps-split] Finish line {metadata.TrackName}: {line.Message} " +
                  $"center {l.Center.Latitude:0.0000000}, {l.Center.Longitude:0.0000000}, length {l.LengthMeters:0} m."
                : $"[gps-split] No finish line: {line.Message}");

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
            Console.WriteLine($"[gps-split] Completed{(dryRun ? " (dry run, nothing written)" : string.Empty)}: " +
                              $"{split} {(dryRun ? "to split" : "split")}, {skipped} skipped, {failed} errors.");

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
        /// Command <c>split-laps</c>: splits all runs under <paramref name="path"/> (event, session, device or run folder)
        /// into laps at the GPS finish line; <paramref name="dryRun"/> only shows what would happen; <paramref name="undo"/> restores
        /// the original laps of the last split. Locks the device folders during the work.
        /// </summary>
        public async Task<OracleExitCode> SplitLapsAsync(string path, bool dryRun, bool undo)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"The specified path does not exist: {path}");
                return OracleExitCode.DataNotFound;
            }

            var runs = RunFolderInspector.FindRunFolders(path);
            if (runs.Count == 0)
            {
                Console.WriteLine($"[split-laps] No run folders found in: {path}");
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
                        Console.WriteLine($"[split-laps] {device.Key} is currently being processed by another ORACLE process – skipped.");
                    }
                }

                if (lockedRuns.Count == 0)
                    return OracleExitCode.ResourceLocked;
                var lockedOut = lockedRuns.Count < runs.Count;

                // Roll back interrupted splits under the locks before undo/analysis looks at the run; the dry run
                // only reports them (Analyze). For --undo a recovery counts as successfully undone.
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
                                Console.WriteLine($"[split-laps] ⚠️ {Path.GetFileName(run)}: processing status could not be reset: {ex.Message}");
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

                    Console.WriteLine($"[split-laps] Undo: {restored} run(s) restored, {failedUndo} errors.");
                    return failedUndo > 0 || lockedOut ? OracleExitCode.PartialSuccess : OracleExitCode.Success;
                }

                // One finish line per track: group runs by event folder. Track without side effects from the event folder
                // (the full metadata resolution may create .HED files and is only used as a fallback outside a dry run).
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
                        Console.WriteLine($"[split-laps] Track for {group.Count} run(s) cannot be determined (event folder in format RT37_CHN-Shanghai expected), e.g. {group[0]}.");
                        unresolved += group.Count;
                        continue;
                    }

                    Console.WriteLine($"[split-laps] Track: {metadata.TrackName} ({metadata.CountryCode}), {group.Count} run(s){(dryRun ? ", dry run" : string.Empty)}.");
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
        /// Rolls back interrupted GPS lap splits (pending manifest) of all runs under <paramref name="root"/> under device and
        /// run lock before a writing command discovers runs or checks idempotency. <c>null</c> if nothing is
        /// pending; otherwise the exit code to abort with (originals remain in the backup).
        /// </summary>
        private static OracleExitCode? RecoverInterruptedSplits(string root)
        {
            var recovery = LapSplitStore.RecoverInterruptedLocked(RunFolderInspector.FindRunFolders(root));
            if (recovery.Failed.Count > 0)
            {
                Console.WriteLine($"[process] Interrupted lap split could not be rolled back ({string.Join(", ", recovery.Failed.Select(Path.GetFileName))}) – " +
                                  $"originals in {LapSplitStore.BackupFolderName}, restore manually.");
                return OracleExitCode.GeneralError;
            }
            if (recovery.Locked.Count > 0)
            {
                Console.WriteLine($"[process] Interrupted lap split in locked run ({string.Join(", ", recovery.Locked.Select(Path.GetFileName))}) – try again later.");
                return OracleExitCode.ResourceLocked;
            }

            return null;
        }

        /// <summary>
        /// Restores the original laps of the last split. The processing status is invalidated beforehand (under the run lock);
        /// if that cannot be saved, the run remains unchanged.
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
        /// Marks the status of the run and its device folder as stale so that the next <c>process</c> call
        /// recalculates the changed laps (lap numbers, LapDistance, sVehicle). Throws if the invalidation cannot
        /// be saved – the caller must then not swap the laps.
        /// </summary>
        private static void InvalidateProcessedStatus(string runPath, string reason)
        {
            OracleStatus.MarkOutdated(runPath, reason);
            if (Path.GetDirectoryName(runPath) is { } device)
                OracleStatus.MarkOutdated(device, reason);
            Console.WriteLine($"[split-laps] {Path.GetFileName(runPath)}: processing status reset – next 'process' call recalculates.");
        }

        /// <summary>
        /// Writes the weather during the runs (<see cref="WeatherService"/>) and records the step <c>weather</c> in the status.
        /// Non-fatal: never changes the exit code; failures (e.g. no internet at the track) are caught up on the next call.
        /// </summary>
        private async Task ApplyWeatherAsync(IReadOnlyList<string> runs, RunMetadata metadata, OracleStatus status)
        {
            if (!_config.Weather.Enabled)
            {
                status.SetStep("weather", StepStatus.Skipped, new Dictionary<string, object> { ["message"] = "disabled in oracle-config.json" });
                return;
            }
            var result = await ComputeWeatherAsync(runs, metadata);
            status.SetStep("weather", result.Status, result.ToDetails());
        }

        /// <summary>
        /// For already processed folders: catches up the weather if the step is missing or did not succeed (e.g. no internet
        /// during the first processing). Only the step <c>weather</c> of the existing status is updated.
        /// </summary>
        private async Task CatchUpWeatherAsync(IReadOnlyList<string> runs, string statusFolder)
        {
            if (!_config.Weather.Enabled || OracleStatus.Load(statusFolder) is not { } status)
                return;
            if (status.Steps.TryGetValue("weather", out var step) && step.Status == StepStatus.Success)
                return;
            if (ReadHedMetadata(runs) is not { } metadata)
                return;

            Console.WriteLine("[weather] Catching up the weather for the already processed runs.");
            var result = await ComputeWeatherAsync(runs, metadata);
            status.SetStep("weather", result.Status, result.ToDetails());
            status.Save(statusFolder);
        }

        /// <summary>
        /// Command <c>weather</c>: writes the weather for all runs under <paramref name="path"/> (event, session, device or run),
        /// per device folder under its lock. Runs need a .HED (run <c>process</c> first). Without <paramref name="force"/>, device
        /// folders whose status already contains a successful weather step are skipped.
        /// </summary>
        public async Task<OracleExitCode> UpdateWeatherAsync(string path, bool force)
        {
            Console.WriteLine($"[weather] Path: {path}");
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"The specified path does not exist: {path}");
                return OracleExitCode.DataNotFound;
            }

            var runs = _browse.IsRunLocation(path)
                ? [path]
                : RunFolders.InOrder(_browse.FindRuns(path) ?? Enumerable.Empty<string>());
            if (runs.Count == 0)
            {
                Console.WriteLine($"No valid runs found in: {path}");
                return OracleExitCode.DataNotFound;
            }

            var codes = new List<OracleExitCode>();
            foreach (var device in runs.GroupBy(r => Path.GetDirectoryName(Path.GetFullPath(r))!, StringComparer.OrdinalIgnoreCase))
            {
                var deviceRuns = device.ToList();
                var name = Path.GetFileName(device.Key);
                using var deviceLock = ProcessingLock.TryAcquire(device.Key);
                if (deviceLock is null)
                {
                    Console.WriteLine($"[weather] {device.Key} is currently being processed by another ORACLE process – try again later.");
                    codes.Add(OracleExitCode.ResourceLocked);
                    continue;
                }

                // Status lives in the device folder (rider/session processing) or in the run folder (single-run processing).
                var statusFolders = new[] { device.Key }.Concat(deviceRuns).Where(f => OracleStatus.Load(f) is not null).ToList();
                if (!force && statusFolders.Count > 0 && statusFolders.All(f =>
                        OracleStatus.Load(f)!.Steps.TryGetValue("weather", out var s) && s.Status == StepStatus.Success))
                {
                    Console.WriteLine($"[weather] {name}: weather already written – skipped (--force to fetch again).");
                    codes.Add(OracleExitCode.Success);
                    continue;
                }

                if (ReadHedMetadata(deviceRuns) is not { } metadata)
                {
                    Console.WriteLine($"[weather] {name}: no .HED in the runs – run 'process' first.");
                    codes.Add(OracleExitCode.DataNotFound);
                    continue;
                }

                var result = await ComputeWeatherAsync(deviceRuns, metadata);
                foreach (var folder in statusFolders)
                {
                    var status = OracleStatus.Load(folder)!;
                    status.SetStep("weather", result.Status, result.ToDetails());
                    status.Save(folder);
                }
                Console.WriteLine($"[weather] {name}: {result.Message}.");
                codes.Add(result.Status switch
                {
                    StepStatus.Success => OracleExitCode.Success,
                    StepStatus.Partial => OracleExitCode.PartialSuccess,
                    StepStatus.Skipped => OracleExitCode.DataNotFound,
                    _ => OracleExitCode.GeneralError,
                });
            }
            return OracleExitCodes.Combine(codes);
        }

        /// <summary>Track coordinate from KOMPASS and the weather for the runs; exceptions become a failed result.</summary>
        private async Task<WeatherStepResult> ComputeWeatherAsync(IReadOnlyList<string> runs, RunMetadata metadata)
        {
            try
            {
                var location = await ResolveFinishLineAsync(metadata, "[weather]");
                var service = new WeatherService(_config.Weather, new OpenMeteoWeatherSource(_config.Weather));
                return await service.ApplyAsync(runs, location);
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                Console.WriteLine($"[weather] ⚠️ Weather not written: {ex.Message}");
                return new WeatherStepResult(StepStatus.Failed, ex.Message, []);
            }
        }

        /// <summary>Metadata (track, country) from the first .HED of the runs, or <c>null</c>.</summary>
        private static RunMetadata? ReadHedMetadata(IReadOnlyList<string> runs)
        {
            var hedFile = runs
                .Where(Directory.Exists)
                .Select(r => Directory.EnumerateFiles(r, "*.HED", SearchOption.TopDirectoryOnly).FirstOrDefault())
                .FirstOrDefault(f => f is not null);
            return hedFile is null ? null : new HedFileService().ReadMetadata(hedFile);
        }

        /// <summary>
        /// Determines the track coordinate (reference for the finish line) from KOMPASS – spelling-tolerant, country preferred.
        /// Returns <c>null</c> if KOMPASS is missing or the track has no lat/lon values.
        /// </summary>
        private async Task<GeoCoordinate?> ResolveFinishLineAsync(RunMetadata metadata, string logPrefix = "[gps-split]")
        {
            if (string.IsNullOrWhiteSpace(metadata.TrackName))
                return null;

            if (!Directory.Exists(_config.DatabaseDirectory))
            {
                Console.WriteLine($"{logPrefix} KOMPASS directory not found: {_config.DatabaseDirectory}");
                return null;
            }

            var kompass = new KompassService();
            if (!await kompass.LoadAsync(_config.DatabaseFilePath))
                return null;

            return kompass.FindFinishLineByTrack(metadata.TrackName, metadata.CountryCode);
        }

        /// <summary>
        /// Supplements missing track metadata (currently: surface) from KOMPASS if not present
        /// in the .HED. Loads KOMPASS only when needed (surface empty and track name known). Non-fatal.
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
        /// Canonically renames device folders with spaces in the name (e.g. <c>"AA 01"</c> → <c>"AA01"</c>),
        /// so that the bikeName is consistently without spaces in folders and XML.
        /// Detects device folders as the direct parents of the found <c>Run_*</c> folders and thus works
        /// on rider as well as session/track level. Non-fatal and guarded.
        /// </summary>
        /// <returns>The possibly updated <paramref name="root"/> path (if it was renamed itself).</returns>
        private string NormalizeDeviceFolders(string root)
        {
            try
            {
                // Determine device folders (direct parents of Run_* folders) purely via System.IO –
                // NOT via KtmDataHandler, whose native directory handles would otherwise block the rename
                // with "Access denied". Deepest first, so that nested
                // renames do not invalidate the paths.
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
                        result = renamed; // root was itself the device folder
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[normalize] ⚠️ Device folder normalization skipped: {ex.Message}");
                return root;
            }
        }

        /// <summary>
        /// Determines all device folders under <paramref name="root"/> purely via System.IO:
        /// folders that directly contain a <c>Run_*</c> subfolder. Does not descend into run/lap folders,
        /// so that a per-run call (KRAKEN) does not rename the parent folder.
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
                    found.Add(dir); // device folder – do not descend further
                else
                    foreach (var s in subDirs) queue.Enqueue(s);
            }
            return found;
        }

        /// <summary>
        /// Canonically renames a single device folder (<c>"AA 01"</c> → <c>"AA01"</c>).
        /// Non-fatal and guarded: only for a non-canonical name and if the target folder is free.
        /// </summary>
        /// <returns>The new path on successful rename, otherwise <c>null</c>.</returns>
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
                Console.WriteLine($"[normalize] Target folder already exists – skipping rename \"{name}\" → \"{canonical}\".");
                return null;
            }

            try
            {
                Directory.Move(deviceFolder, target);
                Console.WriteLine($"[normalize] Device folder renamed: \"{name}\" → \"{canonical}\".");
                return target;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[normalize] ⚠️ Rename failed for \"{name}\": {ex.Message}. Processing with original name.");
                return null;
            }
        }

        /// <summary>
        /// Determines the run metadata from a HED file or the KOMPASS database.
        /// </summary>
        /// <returns>The metadata (or <c>null</c>) and whether an existing KOMPASS.json was invalid (exit 7 instead of 3).</returns>
        private async Task<(RunMetadata? Metadata, bool KompassInvalid)> ResolveMetadataAsync(string runPath)
        {
            var riderPath = Path.GetFullPath(Path.Combine(runPath, ".."));
            Console.WriteLine($"Rider Path: {riderPath}");

            if (!Directory.Exists(riderPath))
            {
                Console.WriteLine($"Rider path does not exist: {riderPath}");
                return (null, false);
            }

            RunMetadata metadata;
            var hedService = new HedFileService();

            // --- Attempt 1: look for .HED file ---
            var hedFilePath = hedService.FindAndPrepareHedFile(runPath, riderPath);
            if (hedFilePath != null)
            {
                Console.WriteLine("Loading metadata from .HED file.");
                metadata = hedService.ReadMetadata(hedFilePath);
                EnrichPathMetadata(metadata, runPath);
                await CompleteFromKompassAsync(metadata, runPath);
            }
            else
            {
                // --- Attempt 2: KOMPASS database ---
                Console.WriteLine("No .HED file found in the rider or run folder.");
                Console.WriteLine("Loading KOMPASS...");

                if (!Directory.Exists(_config.DatabaseDirectory))
                {
                    Console.WriteLine($"Database directory not found: {_config.DatabaseDirectory}");
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
        /// Supplements metadata read from a .HED with missing fields from path + KOMPASS (e.g. empty riderName
        /// or team/season/discipline from an older .HED). Existing .HED values remain unchanged. Non-fatal.
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
                    ? $"[metadata] ⚠️ {d.Field}: .HED \"{d.HedValue}\" differs from KOMPASS \"{d.KompassValue}\" – .HED value kept (PreferHedMetadata)."
                    : $"[metadata] {d.Field}: .HED \"{d.HedValue}\" → \"{d.KompassValue}\" (KOMPASS/path).");
            }
        }

        /// <summary>
        /// Fills missing path metadata needed for the .HED file name
        /// (SeasonName, EventName, SessionName, BikeName) from the run path.
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
        /// Finds valid WinTAX4 runs via the IMarelliDataBrowse API.
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
