using Ktm.Racing.Domain.Geo;
using Ktm.Racing.Domain.Telemetry;
using KtmDataHandler.Marelli.Wintax;

namespace Ktm.Racing.Oracle;

/// <summary>Outcome category of a GPS lap split on a single run.</summary>
public enum GpsLapSplitOutcomeStatus
{
    /// <summary>The run was split and the new laps were swapped in (originals backed up).</summary>
    Split,

    /// <summary>Dry run: the run would be split as reported; nothing was written.</summary>
    Planned,

    /// <summary>Nothing was written (already split, no GPS, no crossing …); non-fatal.</summary>
    Skipped,

    /// <summary>An error occurred; the run is unchanged (rolled back).</summary>
    Failed,
}

/// <summary>The result of splitting one run by GPS.</summary>
public sealed class GpsLapSplitOutcome
{
    /// <summary>Run folder.</summary>
    public required string RunPath { get; init; }

    /// <summary>Outcome category.</summary>
    public required GpsLapSplitOutcomeStatus Status { get; init; }

    /// <summary>Detector verdict (null when the run could not be read).</summary>
    public LapSplitVerdict? Verdict { get; init; }

    /// <summary>Number of laps written or planned.</summary>
    public int LapCount { get; init; }

    /// <summary>Lap marks written or planned (s).</summary>
    public IReadOnlyList<double> Marks { get; init; } = [];

    /// <summary>Largest time shift of a verified channel caused by the rewrite (ms).</summary>
    public double MaxTimeShiftMs { get; init; }

    /// <summary>Largest sample-count difference of a verified channel.</summary>
    public int MaxSampleDelta { get; init; }

    /// <summary>For runs that are already split: largest distance (s) between an existing boundary and the nearest GPS crossing.</summary>
    public double? ExistingBoundaryDeviationSeconds { get; init; }

    /// <summary>Backup folder of the original laps (only for <see cref="GpsLapSplitOutcomeStatus.Split"/>).</summary>
    public string? BackupFolder { get; init; }

    /// <summary>Human-readable summary.</summary>
    public string Message { get; init; } = string.Empty;
}

/// <summary>GPS and lap structure of one run, read before the finish line is placed (phase 1 of a split).</summary>
public sealed class RunSplitAnalysis
{
    /// <summary>Run folder.</summary>
    public required string RunPath { get; init; }

    /// <summary>Lap marks of the run as stored today (s).</summary>
    public IReadOnlyList<double> Marks { get; init; } = [];

    /// <summary>Laps of the run with their acquisition and time span (empty when the structure is inconsistent).</summary>
    public IReadOnlyList<RecordedLap> Laps { get; init; } = [];

    /// <summary>GPS recording; null when not available.</summary>
    public GpsTrace? Gps { get; init; }

    /// <summary>Why the run cannot be analyzed (null when fine).</summary>
    public string? Problem { get; init; }
}

/// <summary>Statistics of a channel used to verify a rewrite.</summary>
/// <param name="Name">Channel name.</param>
/// <param name="Count">Number of samples.</param>
/// <param name="FirstTime">Time of the first sample (s).</param>
/// <param name="Rate">Sample rate (Hz), rounded.</param>
public sealed record ChannelStat(string Name, int Count, double FirstTime, double Rate);

/// <summary>Result of <see cref="ISplitDataAccess.WriteSplit"/>.</summary>
/// <param name="Errors">Write errors reported by the API (empty when fine).</param>
/// <param name="Channels">Statistics of all channels of the original run.</param>
public sealed record SplitWriteResult(IReadOnlyList<string> Errors, IReadOnlyList<ChannelStat> Channels);

/// <summary>Marks and channel statistics read back from a run.</summary>
/// <param name="Marks">Lap marks (s).</param>
/// <param name="ChannelCount">Number of channels in the run.</param>
/// <param name="Channels">Statistics of the requested channels.</param>
public sealed record SplitReadBack(IReadOnlyList<double> Marks, int ChannelCount, IReadOnlyList<ChannelStat> Channels);

/// <summary>Access to WinTAX run data needed for a split (native API in production, fake in tests).</summary>
public interface ISplitDataAccess
{
    /// <summary>Reads lap marks and the GPS channels; <c>Gps</c> is null with a reason when they are missing.</summary>
    (IReadOnlyList<double> Marks, GpsTrace? Gps, string? Problem) ReadGps(string runPath, string latitudeChannel, string longitudeChannel);

    /// <summary>Loads all channels of <paramref name="runPath"/> and writes them split at <paramref name="marks"/> under <paramref name="outputDirectory"/>.</summary>
    SplitWriteResult WriteSplit(string runPath, IReadOnlyList<double> marks, string outputDirectory);

    /// <summary>Reads marks and statistics of <paramref name="channels"/> of a run.</summary>
    SplitReadBack ReadBack(string runPath, IReadOnlyCollection<string> channels);
}

/// <summary>
/// Splits WinTAX runs into laps at a GPS finish line, headlessly and safely:
/// <list type="number">
/// <item>Read GPS and lap structure of all runs (<see cref="Analyze"/>).</item>
/// <item>Place one finish line for all runs of the track from the KOMPASS coordinate (<see cref="FinishLineBuilder"/>).</item>
/// <item>Per run: detect whether it already has real laps (<see cref="LapSplitDetector"/>) – if so, nothing happens.</item>
/// <item>Compute the crossings, write the laps to a temporary folder, verify them, swap them in with a backup of the
/// original laps (<see cref="LapSplitStore"/>), verify again; any failure rolls back.</item>
/// </list>
/// Never throws per run; a bad run never aborts the batch.
/// </summary>
public sealed class GpsLapSplitService
{
    private const double MarkTolerance = 0.002;
    private readonly GpsLapSplitSettings _settings;
    private readonly GpsSettings _gps;
    private readonly ISplitDataAccess _access;
    private readonly Func<string, long?> _freeSpace;
    private readonly bool _lockRuns;
    private readonly Action<string>? _beforeSwap;
    private readonly GpsLapSplitter _splitter = new();

    /// <summary>Creates the service.</summary>
    /// <param name="settings">Configuration (oracle-config.json, section GpsLapSplit).</param>
    /// <param name="access">Data access; default is the native WinTAX API.</param>
    /// <param name="freeSpace">Free bytes on the volume of a path (null = unknown); default uses <see cref="DriveInfo"/>.</param>
    /// <param name="lockRuns">
    /// Lock each run folder (<c>oracle.lock</c>) while writing – for the stand-alone command; <c>process</c> already holds the lock.
    /// </param>
    /// <param name="gps">GPS channels and preparation (oracle-config.json, section Gps).</param>
    /// <param name="beforeSwap">
    /// Called with the run path under the run lock right before the laps are swapped (e.g. to persist the invalidation of
    /// processing statuses). If it throws, the run is left unchanged and the split fails.
    /// </param>
    public GpsLapSplitService(GpsLapSplitSettings? settings = null, ISplitDataAccess? access = null, Func<string, long?>? freeSpace = null,
        bool lockRuns = false, GpsSettings? gps = null, Action<string>? beforeSwap = null)
    {
        _settings = settings ?? new GpsLapSplitSettings();
        _gps = gps ?? new GpsSettings();
        _access = access ?? new KtmSplitDataAccess();
        _freeSpace = freeSpace ?? FreeSpace;
        _lockRuns = lockRuns;
        _beforeSwap = beforeSwap;
    }

    /// <summary>
    /// Phase 1: reads GPS (prepared by <see cref="GpsCleaner"/>) and lap structure of a run. Never throws and never writes: a run
    /// with an interrupted lap swap is only reported – callers recover it beforehand under the processing locks
    /// (<see cref="LapSplitStore.RecoverInterruptedLocked"/>).
    /// </summary>
    public RunSplitAnalysis Analyze(string runPath)
    {
        try
        {
            if (LapSplitStore.HasInterruptedSplit(runPath))
                return new RunSplitAnalysis
                {
                    RunPath = runPath,
                    Problem = "an interrupted lap split must be recovered first (originals in " +
                              $"{LapSplitStore.BackupFolderName}) – run 'process' or 'split-laps' without --dry-run",
                };

            var (marks, raw, problem) = _access.ReadGps(runPath, _gps.LatitudeChannel, _gps.LongitudeChannel);
            var cleaned = raw is null ? null : GpsCleaner.Clean(raw, _gps.ToCleanOptions());
            var gps = cleaned is { HasFix: true } ? cleaned.Trace : null;
            if (raw is not null && gps is null)
                problem = "no GPS fix in this run";
            var folders = LapSplitStore.ReadLapFolders(runPath);
            if (marks.Count < 2)
                return new RunSplitAnalysis { RunPath = runPath, Marks = marks, Gps = gps, Problem = problem ?? "the run has no lap marks" };
            if (folders.Count != marks.Count - 1)
            {
                return new RunSplitAnalysis
                {
                    RunPath = runPath, Marks = marks, Gps = gps,
                    Problem = $"lap structure is inconsistent ({folders.Count} lap folder(s), {marks.Count - 1} lap(s) in the data) – repair in WinTAX",
                };
            }

            var laps = folders.Select((f, i) => new RecordedLap(f.Folder, f.Acquisition, marks[i], marks[i + 1])).ToList();
            return new RunSplitAnalysis { RunPath = runPath, Marks = marks, Laps = laps, Gps = gps, Problem = gps is null ? problem : null };
        }
        catch (Exception ex)
        {
            return new RunSplitAnalysis { RunPath = runPath, Problem = $"run could not be read: {ex.Message}" };
        }
    }

    /// <summary>Phase 2: one finish line for all analyzed runs of a track.</summary>
    public FinishLineBuildResult BuildFinishLine(GeoCoordinate trackCoordinate, IEnumerable<RunSplitAnalysis> analyses) =>
        FinishLineBuilder.Build(trackCoordinate, analyses.Select(a => a.Gps).OfType<GpsTrace>(), _settings.ToLineOptions());

    /// <summary>Phase 3: decides and – unless <paramref name="dryRun"/> – splits one run. Never throws.</summary>
    public GpsLapSplitOutcome SplitRun(RunSplitAnalysis analysis, FinishLine line, bool dryRun = false)
    {
        ArgumentNullException.ThrowIfNull(analysis);
        ArgumentNullException.ThrowIfNull(line);
        var run = analysis.RunPath;

        if (analysis.Laps.Count == 0)
            return Skipped(run, null, analysis.Problem ?? "no laps");

        var crossings = analysis.Gps is null ? null : GpsLapSplitter.FindCrossings(analysis.Gps, line, _settings.ToOptions(_gps));
        var detection = LapSplitDetector.Detect(analysis.Laps, analysis.Gps, line, crossings, _settings.ToDetectorOptions());
        if (!detection.ShouldSplit)
        {
            var deviation = detection.Verdict == LapSplitVerdict.AlreadySplit && crossings is { Count: > 0 }
                ? analysis.Laps.Skip(1).Select(l => crossings.Min(c => Math.Abs(c.Time - l.Start))).DefaultIfEmpty(0).Max()
                : (double?)null;
            var note = deviation is { } d ? $" (existing boundaries vs. GPS line: max {d:0.00} s)" : string.Empty;
            return new GpsLapSplitOutcome
            {
                RunPath = run,
                Status = GpsLapSplitOutcomeStatus.Skipped,
                Verdict = detection.Verdict,
                ExistingBoundaryDeviationSeconds = deviation,
                Message = $"{Describe(detection.Verdict)}: {detection.Reason}{note} – not split",
            };
        }

        if (analysis.Gps is null)
            return Skipped(run, detection.Verdict, analysis.Problem ?? "no GPS");

        var split = _splitter.ComputeLapMarks(analysis.Gps, line, _settings.ToOptions(_gps), analysis.Marks[0], analysis.Marks[^1]);
        if (split.Status != GpsLapSplitStatus.Success)
            return Skipped(run, detection.Verdict, split.Status == GpsLapSplitStatus.NoCrossings
                ? $"{Describe(detection.Verdict)}, no lap to separate: {split.Message}"
                : $"{Describe(detection.Verdict)}, cannot split: {split.Message}");

        var marksText = string.Join(", ", split.LapMarks.Select(m => m.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture)));
        if (dryRun)
        {
            return new GpsLapSplitOutcome
            {
                RunPath = run,
                Status = GpsLapSplitOutcomeStatus.Planned,
                Verdict = detection.Verdict,
                LapCount = split.LapCount,
                Marks = split.LapMarks,
                Message = $"{Describe(detection.Verdict)} ({detection.Reason}) → would split into {split.LapCount} lap(s) [{marksText}]",
            };
        }

        return Write(analysis, detection, split, line, marksText);
    }

    /// <summary>Analyzes all runs, places one finish line and splits each run. Never throws.</summary>
    public (FinishLineBuildResult Line, IReadOnlyList<GpsLapSplitOutcome> Outcomes) SplitRuns(
        IReadOnlyList<string> runs, GeoCoordinate trackCoordinate, bool dryRun = false)
    {
        var analyses = runs.Select(Analyze).ToList();
        var line = BuildFinishLine(trackCoordinate, analyses);
        if (line.Line is null)
            return (line, analyses.Select(a => Skipped(a.RunPath, null, line.Message)).ToList());

        return (line, analyses.Select(a => SplitRun(a, line.Line, dryRun)).ToList());
    }

    private GpsLapSplitOutcome Write(RunSplitAnalysis analysis, LapSplitDetection detection, GpsLapSplitResult split, FinishLine line, string marksText)
    {
        var run = analysis.RunPath;
        var temp = Path.Combine(run, LapSplitStore.TempFolderName);
        using var runLock = _lockRuns ? ProcessingLock.TryAcquire(run) : null;
        if (_lockRuns && runLock is null)
            return Failed(run, detection.Verdict, "the run is being processed by another ORACLE process – try again later");
        var committed = false;
        try
        {
            if (LapSplitStore.HasInterruptedSplit(run))
                return Failed(run, detection.Verdict, "an interrupted lap split must be recovered first – nothing changed");

            if (LapSplitStore.CleanupTemp(run))
                Console.WriteLine($"[gps-split] {Path.GetFileName(run)}: leftover {LapSplitStore.TempFolderName} from an aborted split removed.");

            var size = LapSplitStore.LapDataSize(run);
            var required = (long)(size * _settings.FreeSpaceFactor) + 64L * 1024 * 1024;
            if (_freeSpace(run) is { } free && free < required)
                return Failed(run, detection.Verdict, $"not enough disk space ({free / 1048576} MB free, {required / 1048576} MB needed) – nothing changed");

            Directory.CreateDirectory(temp);
            var write = _access.WriteSplit(run, split.LapMarks, temp);
            if (write.Errors.Count > 0)
                return Failed(run, detection.Verdict, $"write API reported errors – nothing changed: {string.Join("; ", write.Errors.Take(3))}");

            var written = LapSplitStore.FindWrittenRun(temp);
            if (written is null)
                return Failed(run, detection.Verdict, "the write API produced no run – nothing changed");

            var verify = VerifyWritten(written, split.LapMarks, write.Channels);
            if (verify.Error is not null)
                return Failed(run, detection.Verdict, $"verification of the written laps failed – nothing changed: {verify.Error}");

            try
            {
                _beforeSwap?.Invoke(run);
            }
            catch (Exception ex)
            {
                return Failed(run, detection.Verdict, $"processing status could not be invalidated – nothing changed: {ex.Message}");
            }

            var manifest = LapSplitStore.Commit(run, written, split.LapMarks, analysis.Laps, new SplitManifest
            {
                Tool = $"ORACLE {typeof(GpsLapSplitService).Assembly.GetName().Version}",
                Verdict = detection.Verdict.ToString(),
                FinishLine = $"{line.A.Latitude:0.0000000},{line.A.Longitude:0.0000000} – {line.B.Latitude:0.0000000},{line.B.Longitude:0.0000000}",
                MaxTimeShiftMs = verify.MaxShiftMs,
                MaxSampleDelta = verify.MaxSampleDelta,
            });
            committed = true;
            var (backup, _) = LapSplitStore.FindLatestSplit(run);

            var after = _access.ReadBack(run, []);
            if (!SameMarks(after.Marks, split.LapMarks))
            {
                var undo = LapSplitStore.Undo(run);
                return Failed(run, detection.Verdict, $"the split run reads back {after.Marks.Count - 1} lap(s) instead of {split.LapCount} – " +
                                                      (undo.Success ? "rolled back" : $"ROLLBACK FAILED: {undo.Message}"));
            }

            return new GpsLapSplitOutcome
            {
                RunPath = run,
                Status = GpsLapSplitOutcomeStatus.Split,
                Verdict = detection.Verdict,
                LapCount = split.LapCount,
                Marks = split.LapMarks,
                MaxTimeShiftMs = verify.MaxShiftMs,
                MaxSampleDelta = verify.MaxSampleDelta,
                BackupFolder = backup,
                Message = $"{Describe(detection.Verdict)} → split into {split.LapCount} lap(s) [{marksText}]; channels re-aligned to their sample grid " +
                          $"(≤ {verify.MaxShiftPeriods:0.00} sample period, GPS {verify.GpsShiftMs:0} ms, slowest channel {verify.MaxShiftMs:0} ms), " +
                          $"sample difference ≤ {verify.MaxSampleDelta}; originals in {Path.GetRelativePath(run, backup ?? run)} ({manifest.OriginalLaps.Count} lap folder(s))",
            };
        }
        catch (Exception ex)
        {
            if (!committed)
                return Failed(run, detection.Verdict, $"{ex.Message} – run unchanged");

            // The new laps are already in place: undo them so the run is never left in an unverified state.
            SplitUndoResult undo;
            try
            {
                undo = LapSplitStore.Undo(run);
            }
            catch (Exception undoError)
            {
                undo = new SplitUndoResult(false, undoError.Message);
            }

            return Failed(run, detection.Verdict, $"{ex.Message} after the laps were swapped – " +
                                                  (undo.Success ? "rolled back" : $"ROLLBACK FAILED ({undo.Message}), originals in {LapSplitStore.BackupFolderName}"));
        }
        finally
        {
            try
            {
                LapSplitStore.CleanupTemp(run);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                Console.WriteLine($"[gps-split] ⚠️ {temp} could not be removed: {ex.Message}");
            }
        }
    }

    /// <summary>Reads the written run back: marks must match; one channel per sample rate is compared with the original.</summary>
    private (string? Error, double MaxShiftMs, int MaxSampleDelta, double MaxShiftPeriods, double GpsShiftMs) VerifyWritten(
        string writtenRun, IReadOnlyList<double> marks, IReadOnlyList<ChannelStat> original)
    {
        var probe = original
            .Where(c => c.Count > 1)
            .GroupBy(c => c.Rate)
            .Select(g => g.First().Name)
            .Append(_gps.LatitudeChannel)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var back = _access.ReadBack(writtenRun, probe);
        if (!SameMarks(back.Marks, marks))
            return ($"{back.Marks.Count - 1} lap(s) read back instead of {marks.Count - 1}", 0, 0, 0, 0);
        if (original.Count > 0 && back.ChannelCount != original.Count)
            return ($"{back.ChannelCount} channel(s) read back instead of {original.Count}", 0, 0, 0, 0);

        double maxShift = 0, maxPeriods = 0, gpsShift = 0;
        var maxDelta = 0;
        var byName = original.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
        foreach (var channel in back.Channels)
        {
            if (!byName.TryGetValue(channel.Name, out var before))
                continue;
            var shift = Math.Abs(channel.FirstTime - before.FirstTime);
            maxShift = Math.Max(maxShift, shift * 1000.0);
            if (before.Rate > 0)
                maxPeriods = Math.Max(maxPeriods, shift * before.Rate);
            if (channel.Name.Equals(_gps.LatitudeChannel, StringComparison.OrdinalIgnoreCase))
                gpsShift = shift * 1000.0;
            maxDelta = Math.Max(maxDelta, Math.Abs(channel.Count - before.Count));
        }

        var allowedDelta = marks.Count + 1;
        if (maxDelta > allowedDelta)
            return ($"a channel lost or gained {maxDelta} samples (allowed {allowedDelta})", maxShift, maxDelta, maxPeriods, gpsShift);
        if (maxPeriods > _settings.MaxTimeShiftSamples + 1e-6)
            return ($"channel time shift of {maxPeriods:0.00} sample periods exceeds MaxTimeShiftSamples {_settings.MaxTimeShiftSamples:0.##}",
                maxShift, maxDelta, maxPeriods, gpsShift);
        return (null, maxShift, maxDelta, maxPeriods, gpsShift);
    }

    private static bool SameMarks(IReadOnlyList<double> actual, IReadOnlyList<double> expected) =>
        actual.Count == expected.Count && actual.Zip(expected).All(p => Math.Abs(p.First - p.Second) <= MarkTolerance);

    private static string Describe(LapSplitVerdict? verdict) => verdict switch
    {
        LapSplitVerdict.NotSplit => "not split yet",
        LapSplitVerdict.AlreadySplit => "already split",
        LapSplitVerdict.Segmented => "logger segments",
        LapSplitVerdict.Ambiguous => "unclear lap structure",
        _ => "unknown",
    };

    private static GpsLapSplitOutcome Skipped(string run, LapSplitVerdict? verdict, string message) =>
        new() { RunPath = run, Status = GpsLapSplitOutcomeStatus.Skipped, Verdict = verdict, Message = message };

    private static GpsLapSplitOutcome Failed(string run, LapSplitVerdict? verdict, string message) =>
        new() { RunPath = run, Status = GpsLapSplitOutcomeStatus.Failed, Verdict = verdict, Message = message };

    private static long? FreeSpace(string path)
    {
        try
        {
            var root = Path.GetPathRoot(Path.GetFullPath(path));
            return string.IsNullOrEmpty(root) || root.StartsWith(@"\\", StringComparison.Ordinal) ? null : new DriveInfo(root).AvailableFreeSpace;
        }
        catch (Exception ex) when (ex is IOException or ArgumentException or UnauthorizedAccessException)
        {
            return null;
        }
    }
}

/// <summary><see cref="ISplitDataAccess"/> on the native WinTAX API (KtmDataHandler).</summary>
public sealed class KtmSplitDataAccess : ISplitDataAccess
{
    /// <inheritdoc />
    public (IReadOnlyList<double> Marks, GpsTrace? Gps, string? Problem) ReadGps(string runPath, string latitudeChannel, string longitudeChannel)
    {
        var access = KtmDataHandlerFactory.CreateAccess();
        try
        {
            access.SetDataLocation([runPath]);
            if (access.LoadRun() <= 0)
                return ([], null, "LoadRun failed");

            var marks = access.GetLapMarks() ?? [];
            var names = access.GetChannelsNames() ?? [];
            var lat = names.FirstOrDefault(n => n.Equals(latitudeChannel, StringComparison.OrdinalIgnoreCase));
            var lon = names.FirstOrDefault(n => n.Equals(longitudeChannel, StringComparison.OrdinalIgnoreCase));
            if (lat is null || lon is null)
                return (marks, null, $"GPS channels {latitudeChannel}/{longitudeChannel} not found");

            var dataset = access.GetChannels([lat, lon]);
            var latSignal = dataset?.GetElement(lat)?.SignalData;
            var lonSignal = dataset?.GetElement(lon)?.SignalData;
            if (latSignal?.Data is not double[] la || lonSignal?.Data is not double[] lo || latSignal.Time is not double[] time)
                return (marks, null, "GPS data not in the expected format");
            if (lo.Length != la.Length || time.Length != la.Length)
                return (marks, null, $"{latitudeChannel} and {longitudeChannel} have different sample counts");

            return (marks, new GpsTrace(time, la, lo), null);
        }
        finally
        {
            access.ReleaseObjects();
        }
    }

    /// <inheritdoc />
    public SplitWriteResult WriteSplit(string runPath, IReadOnlyList<double> marks, string outputDirectory)
    {
        var reader = KtmDataHandlerFactory.CreateAccess();
        var writer = KtmDataHandlerFactory.CreateWriter();
        try
        {
            reader.SetDataLocation([runPath]);
            if (reader.LoadRun() <= 0)
                return new SplitWriteResult(["LoadRun failed"], []);

            var names = reader.GetChannelsNames() ?? [];
            var dataset = reader.GetChannels(names);
            if (dataset is null || dataset.NumElements() == 0)
                return new SplitWriteResult(["no channels loaded"], []);

            var stats = Stats(dataset, names);

            // Lap start offset in ms (sub-second part of each lap start); without it the API rejects slow channels.
            var startOffsets = marks.Take(marks.Count - 1)
                .Select(m => (ushort)Math.Clamp((int)Math.Round((m - Math.Floor(m)) * 1000.0), 0, 999))
                .ToArray();
            writer.SetDataLocation([outputDirectory]);
            var errors = KtmWriteErrors.Describe(writer.SetChannelsSplitByLap(dataset, [.. marks], startOffsets), "lap");
            return new SplitWriteResult(errors, stats);
        }
        finally
        {
            writer.ReleaseObjects();
            reader.ReleaseObjects();
        }
    }

    /// <inheritdoc />
    public SplitReadBack ReadBack(string runPath, IReadOnlyCollection<string> channels)
    {
        var access = KtmDataHandlerFactory.CreateAccess();
        try
        {
            access.SetDataLocation([runPath]);
            if (access.LoadRun() <= 0)
                return new SplitReadBack([], 0, []);

            var marks = access.GetLapMarks() ?? [];
            var names = access.GetChannelsNames() ?? [];
            var wanted = names.Where(n => channels.Contains(n, StringComparer.OrdinalIgnoreCase)).ToArray();
            var stats = wanted.Length == 0 ? [] : Stats(access.GetChannels(wanted), wanted);
            return new SplitReadBack(marks, names.Length, stats);
        }
        finally
        {
            access.ReleaseObjects();
        }
    }

    private static List<ChannelStat> Stats(KtmDataHandler.Models.Channels.IDataset? dataset, IEnumerable<string> names)
    {
        var result = new List<ChannelStat>();
        if (dataset is null)
            return result;
        foreach (var name in names)
        {
            if (dataset.GetElement(name)?.SignalData?.Time is not double[] time || time.Length == 0)
                continue;
            var rate = time.Length > 1 && time[^1] > time[0] ? Math.Round((time.Length - 1) / (time[^1] - time[0])) : 0;
            result.Add(new ChannelStat(name, time.Length, time[0], rate));
        }

        return result;
    }
}
