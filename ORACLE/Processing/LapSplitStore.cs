using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using Ktm.Racing.Domain.Telemetry;

namespace Ktm.Racing.Oracle;

/// <summary>Record of a GPS lap split, stored next to the backed-up original laps (<c>split-manifest.json</c>).</summary>
public sealed class SplitManifest
{
    /// <summary>Format version.</summary>
    public int Version { get; set; } = 1;

    /// <summary>When the split was committed (UTC).</summary>
    public DateTime CreatedUtc { get; set; }

    /// <summary>Tool and version that wrote the split.</summary>
    public string Tool { get; set; } = string.Empty;

    /// <summary>Detector verdict that allowed the split.</summary>
    public string Verdict { get; set; } = string.Empty;

    /// <summary>Lap marks written (s).</summary>
    public List<double> Marks { get; set; } = [];

    /// <summary>Lap folders that were moved into the backup.</summary>
    public List<string> OriginalLaps { get; set; } = [];

    /// <summary>Lap folders created by the split.</summary>
    public List<string> CreatedLaps { get; set; } = [];

    /// <summary><c>true</c> when RunLapHeader.xml existed and was backed up.</summary>
    public bool HadRunLapHeader { get; set; }

    /// <summary>Finish line used (text, for traceability).</summary>
    public string FinishLine { get; set; } = string.Empty;

    /// <summary>Largest time shift of a channel caused by the rewrite (ms).</summary>
    public double MaxTimeShiftMs { get; set; }

    /// <summary>Largest sample-count difference of a channel caused by the rewrite.</summary>
    public int MaxSampleDelta { get; set; }

    /// <summary><c>true</c> once the split was undone.</summary>
    public bool Undone { get; set; }

    /// <summary>When the split was undone (UTC).</summary>
    public DateTime? UndoneUtc { get; set; }
}

/// <summary>A lap folder of a run in run order with the acquisition it belongs to.</summary>
/// <param name="Folder">Lap folder name.</param>
/// <param name="Acquisition">WinTAX <c>Abs</c> (acquisition number), empty when unknown.</param>
public sealed record LapFolderInfo(string Folder, string Acquisition);

/// <summary>Result of <see cref="LapSplitStore.Undo"/>.</summary>
/// <param name="Success"><c>true</c> when the original laps were restored.</param>
/// <param name="Message">Human-readable result.</param>
public sealed record SplitUndoResult(bool Success, string Message);

/// <summary>
/// File side of the GPS lap split (no native API, testable): reads the lap structure of a run and swaps the laps written by
/// the WinTAX API into the run as a transaction – originals are moved (not copied) to <c>_ORACLE_original\&lt;timestamp&gt;</c>,
/// new laps get WinTAX names (<c>…_Def_000</c>), complete LapHeader.xml/RunLapHeader.xml and the per-lap files the API does
/// not write. Any failure rolls back; <see cref="Undo"/> restores the originals later.
/// </summary>
public static class LapSplitStore
{
    /// <summary>Temporary output folder inside the run (same volume → moves, no copies).</summary>
    public const string TempFolderName = "_ORACLE_split_tmp";

    /// <summary>Backup folder inside the run.</summary>
    public const string BackupFolderName = "_ORACLE_original";

    /// <summary>Manifest file name inside a backup.</summary>
    public const string ManifestFileName = "split-manifest.json";

    private const string RunLapHeaderFile = "RunLapHeader.xml";
    private const string LapHeaderFile = "LapHeader.xml";
    private const string DataFile = "cableData.ztx";
    private const string WallClockFormat = "dd/MM/yyyy HH:mm:ss.fff";

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    /// <summary>
    /// The lap folders of a run in run order (RunLapHeader.xml; without it: <c>Lap_*</c> by name) with their acquisition.
    /// </summary>
    public static IReadOnlyList<LapFolderInfo> ReadLapFolders(string runPath)
    {
        var folders = new List<string>();
        var header = Path.Combine(runPath, RunLapHeaderFile);
        if (File.Exists(header))
        {
            try
            {
                folders = XDocument.Load(header).Descendants("LapInfo")
                    .Select(info => FirstSegment(info.Element("Path")?.Value))
                    .Where(name => IsSafeLapFolderName(name) && Directory.Exists(Path.Combine(runPath, name)))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch (Exception ex) when (ex is IOException or System.Xml.XmlException or UnauthorizedAccessException)
            {
                folders = [];
            }
        }

        if (folders.Count == 0 && Directory.Exists(runPath))
        {
            folders = Directory.GetDirectories(runPath, "Lap_*", SearchOption.TopDirectoryOnly)
                .Where(d => File.Exists(Path.Combine(d, DataFile)))
                .Select(Path.GetFileName)
                .OfType<string>()
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        return folders.Select(f => new LapFolderInfo(f, ReadAcquisition(Path.Combine(runPath, f)))).ToList();
    }

    /// <summary>Total size of the lap data (<c>cableData.ztx</c>) of a run in bytes.</summary>
    public static long LapDataSize(string runPath) =>
        Directory.Exists(runPath)
            ? Directory.GetDirectories(runPath, "Lap_*", SearchOption.TopDirectoryOnly)
                .Select(d => new FileInfo(Path.Combine(d, DataFile)))
                .Where(f => f.Exists)
                .Sum(f => f.Length)
            : 0;

    /// <summary>The single <c>Run_*</c> folder the write API created under <paramref name="tempRoot"/>; <c>null</c> if none or several.</summary>
    public static string? FindWrittenRun(string tempRoot)
    {
        if (!Directory.Exists(tempRoot))
            return null;
        var runs = Directory.GetDirectories(tempRoot, "Run_*", SearchOption.AllDirectories);
        return runs.Length == 1 ? runs[0] : null;
    }

    /// <summary>Removes a leftover temporary folder (e.g. after a crash). Returns <c>true</c> if one was removed.</summary>
    public static bool CleanupTemp(string runPath)
    {
        var temp = Path.Combine(runPath, TempFolderName);
        if (!Directory.Exists(temp))
            return false;
        Retry(() => Directory.Delete(temp, recursive: true));
        return true;
    }

    /// <summary>
    /// Swaps the laps of <paramref name="writtenRun"/> into <paramref name="runPath"/>. <paramref name="originalLaps"/> are the
    /// current laps with their time spans (run order); <paramref name="marks"/> the lap boundaries that were written.
    /// Throws (after rolling back) when anything fails.
    /// </summary>
    public static SplitManifest Commit(string runPath, string writtenRun, IReadOnlyList<double> marks,
        IReadOnlyList<RecordedLap> originalLaps, SplitManifest template)
    {
        ArgumentNullException.ThrowIfNull(template);
        if (marks.Count < 2)
            throw new ArgumentException("At least two marks are required.", nameof(marks));
        if (originalLaps.Count == 0)
            throw new ArgumentException("The run has no laps.", nameof(originalLaps));

        var written = Directory.GetDirectories(writtenRun, "Lap_*", SearchOption.TopDirectoryOnly)
            .OrderBy(d => d, StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (written.Count != marks.Count - 1)
            throw new InvalidDataException($"The write API created {written.Count} lap(s), expected {marks.Count - 1}.");
        var empty = written.FirstOrDefault(d => !(new FileInfo(Path.Combine(d, DataFile)) is { Exists: true, Length: > 0 }));
        if (empty is not null)
            throw new InvalidDataException($"Written lap {Path.GetFileName(empty)} has no data.");

        var plan = PlanNewLaps(marks, originalLaps);
        var backup = CreateBackupFolder(runPath);
        var runLapHeader = Path.Combine(runPath, RunLapHeaderFile);
        var backupRunLapHeader = Path.Combine(backup, RunLapHeaderFile);
        var moved = new List<string>();
        var created = new List<string>();
        var hadRunLapHeader = File.Exists(runLapHeader);

        try
        {
            if (hadRunLapHeader)
                File.Copy(runLapHeader, backupRunLapHeader);

            foreach (var lap in Directory.GetDirectories(runPath, "Lap_*", SearchOption.TopDirectoryOnly))
            {
                var name = Path.GetFileName(lap);
                Retry(() => Directory.Move(lap, Path.Combine(backup, name)));
                moved.Add(name);
            }

            for (var i = 0; i < plan.Count; i++)
            {
                var target = Path.Combine(runPath, plan[i].Name);
                if (Directory.Exists(target))
                    throw new IOException($"Lap folder {plan[i].Name} already exists.");
                Retry(() => Directory.Move(written[i], target));
                created.Add(plan[i].Name);

                var source = Path.Combine(backup, plan[i].Source);
                CopyMissingLapFiles(source, target);
                WriteLapHeader(Path.Combine(source, LapHeaderFile), Path.Combine(target, LapHeaderFile), plan[i]);
            }

            if (hadRunLapHeader)
                WriteRunLapHeader(backupRunLapHeader, runLapHeader, plan);

            template.CreatedUtc = DateTime.UtcNow;
            template.Marks = [.. marks];
            template.OriginalLaps = moved;
            template.CreatedLaps = created;
            template.HadRunLapHeader = hadRunLapHeader;
            WriteManifest(backup, template);
            File.WriteAllText(Path.Combine(backup, "README.txt"),
                $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} ORACLE split this run into {created.Count} lap(s) by GPS finish line.{Environment.NewLine}" +
                $"The original lap folders and RunLapHeader.xml are stored here unchanged.{Environment.NewLine}" +
                $"To restore them run: ORACLE.exe split-laps \"{runPath}\" --undo{Environment.NewLine}");
            return template;
        }
        catch (Exception ex)
        {
            try
            {
                Restore(runPath, backup, moved, created, hadRunLapHeader);
                if (Directory.GetDirectories(backup).Length == 0)
                    Retry(() => Directory.Delete(backup, recursive: true));
            }
            catch (Exception rollback)
            {
                throw new IOException($"{ex.Message} – rollback failed: {rollback.Message}", ex);
            }

            throw;
        }
    }

    /// <summary>Restores the original laps of the most recent split that is not yet undone.</summary>
    public static SplitUndoResult Undo(string runPath)
    {
        var (backup, manifest) = FindLatestSplit(runPath);
        if (backup is null || manifest is null)
            return new SplitUndoResult(false, "no ORACLE lap split to undo");

        var invalid = manifest.CreatedLaps.Concat(manifest.OriginalLaps).FirstOrDefault(n => !IsSafeLapFolderName(n));
        if (invalid is not null)
            return new SplitUndoResult(false, $"manifest contains an invalid folder name ({invalid}) – not touched");

        var current = Directory.GetDirectories(runPath, "Lap_*", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName).OfType<string>().ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!current.SetEquals(manifest.CreatedLaps))
            return new SplitUndoResult(false, "the laps were changed after the split (WinTAX?) – restore manually from " + backup);
        var missing = manifest.OriginalLaps.FirstOrDefault(n => !Directory.Exists(Path.Combine(backup, n)));
        if (missing is not null)
            return new SplitUndoResult(false, $"backup is incomplete ({missing} missing) – not touched");

        Restore(runPath, backup, manifest.OriginalLaps, manifest.CreatedLaps, manifest.HadRunLapHeader);
        manifest.Undone = true;
        manifest.UndoneUtc = DateTime.UtcNow;
        WriteManifest(backup, manifest);
        return new SplitUndoResult(true, $"{manifest.OriginalLaps.Count} original lap(s) restored, {manifest.CreatedLaps.Count} split lap(s) removed");
    }

    /// <summary>The most recent backup with a manifest that has not been undone.</summary>
    public static (string? Backup, SplitManifest? Manifest) FindLatestSplit(string runPath)
    {
        var root = Path.Combine(runPath, BackupFolderName);
        if (!Directory.Exists(root))
            return (null, null);

        foreach (var folder in Directory.GetDirectories(root).OrderByDescending(d => d, StringComparer.OrdinalIgnoreCase))
        {
            var file = Path.Combine(folder, ManifestFileName);
            if (!File.Exists(file))
                continue;
            try
            {
                var manifest = JsonSerializer.Deserialize<SplitManifest>(File.ReadAllText(file));
                if (manifest is { Undone: false })
                    return (folder, manifest);
            }
            catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException)
            {
                // A damaged manifest is never acted upon.
            }
        }

        return (null, null);
    }

    /// <summary>Planned name and timing of a new lap.</summary>
    internal sealed record NewLap(string Name, string Source, double Start, double End, double SourceStart, bool FirstOfSource, bool LastOfSource);

    /// <summary>Names the new laps <c>&lt;source lap&gt;_Def_&lt;i&gt;</c> (WinTAX convention) and maps each to the lap it comes from.</summary>
    internal static List<NewLap> PlanNewLaps(IReadOnlyList<double> marks, IReadOnlyList<RecordedLap> originalLaps)
    {
        var result = new List<NewLap>();
        for (var i = 0; i < marks.Count - 1; i++)
        {
            var start = marks[i];
            var source = originalLaps.LastOrDefault(l => l.Start <= start + 1e-6) ?? originalLaps[0];
            result.Add(new NewLap($"{source.Name}_Def_{i:000}", source.Name, start, marks[i + 1], source.Start, false, false));
        }

        for (var i = 0; i < result.Count; i++)
        {
            result[i] = result[i] with
            {
                FirstOfSource = i == 0 || result[i - 1].Source != result[i].Source,
                LastOfSource = i == result.Count - 1 || result[i + 1].Source != result[i].Source,
            };
        }

        return result;
    }

    /// <summary>
    /// Writes a complete LapHeader.xml for a new lap from the source lap's header (alias, table, serial, acquisition …),
    /// with the new lap time, start time and open/close type. Without a template the API-written header is patched.
    /// </summary>
    internal static void WriteLapHeader(string templatePath, string targetPath, NewLap lap)
    {
        var doc = XDocument.Load(File.Exists(templatePath) ? templatePath : targetPath);
        if (doc.Root is not { } root)
            return;

        var duration = lap.End - lap.Start;
        Set(root, "LapTime", duration.ToString("0.####", CultureInfo.InvariantCulture));
        if (!lap.FirstOfSource)
            Set(root, "OpenType", "PCCommand");
        if (!lap.LastOfSource)
            Set(root, "CloseType", "PcCommand");

        var info = root.Element("StartTimeInfo")?.Element("InfoArray")?.Element("StartTimeInfo");
        if (info is not null && TryShift(info.Element("STMs")?.Value, info.Element("STS")?.Value, info.Element("STNs")?.Value,
                lap.Start - lap.SourceStart, out var wall, out var seconds, out var nanos))
        {
            Set(info, "STMs", wall.ToString(WallClockFormat, CultureInfo.InvariantCulture));
            Set(info, "STS", seconds.ToString(CultureInfo.InvariantCulture));
            Set(info, "STNs", nanos.ToString(CultureInfo.InvariantCulture));
            Set(root, "StartTime", wall.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
        }

        doc.Save(targetPath);
    }

    /// <summary>Builds RunLapHeader.xml for the new laps from the original one (one cloned entry per new lap).</summary>
    internal static void WriteRunLapHeader(string originalPath, string targetPath, IReadOnlyList<NewLap> laps)
    {
        var doc = XDocument.Load(originalPath);
        var lapArray = doc.Root?.Element("Run")?.Element("LapArray")
            ?? throw new InvalidDataException("RunLapHeader.xml has no Run/LapArray.");
        var templates = lapArray.Elements("Lap").ToList();
        if (templates.Count == 0)
            throw new InvalidDataException("RunLapHeader.xml has no lap entries.");

        XElement TemplateFor(string source) =>
            templates.FirstOrDefault(t => string.Equals(FirstSegment(t.Element("LapInfo")?.Element("Path")?.Value), source, StringComparison.OrdinalIgnoreCase))
            ?? templates[0];

        var entries = new List<XElement>();
        foreach (var lap in laps)
        {
            var template = TemplateFor(lap.Source);
            var entry = new XElement(template);
            var lapInfo = entry.Element("LapInfo");
            Set(lapInfo, "Path", $"{lap.Name}\\{DataFile}");
            Set(lapInfo, "LapIndex", "0");

            var infos = entry.Element("Infos");
            var duration = lap.End - lap.Start;
            var ticks = Math.Round(duration * 10000.0).ToString("0", CultureInfo.InvariantCulture);
            SetInfo(infos, "LapTime", ticks);
            SetInfo(infos, "LapTimeMarker", ticks);
            SetInfo(infos, "CronoTime", duration.ToString("0.00", CultureInfo.InvariantCulture));
            SetInfo(infos, "FilePath", $"{lap.Name}\\");
            if (!lap.FirstOfSource)
                SetInfo(infos, "OpenType", "PCCommand");
            if (!lap.LastOfSource)
                SetInfo(infos, "CloseType", "PcCommand");

            var startS = GetInfo(infos, "StartTimeS");
            var startNs = GetInfo(infos, "StartTimeNs");
            if (TryShift(null, startS, startNs, lap.Start - lap.SourceStart, out _, out var seconds, out var nanos))
            {
                SetInfo(infos, "StartTimeS", seconds.ToString(CultureInfo.InvariantCulture));
                SetInfo(infos, "StartTimeNs", nanos.ToString(CultureInfo.InvariantCulture));
                SetInfo(infos, "StartTime", seconds.ToString(CultureInfo.InvariantCulture));
            }

            if (TryParseClock(GetInfo(infos, "StartTimeMSec"), out var clock))
                SetInfo(infos, "StartTimeMSec", clock.Add(TimeSpan.FromSeconds(lap.Start - lap.SourceStart)).ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture));

            entries.Add(entry);
        }

        lapArray.Elements("Lap").Remove();
        lapArray.Add(entries);
        Set(doc.Root?.Element("Inf"), "ModT", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture));

        var temp = targetPath + ".tmp";
        doc.Save(temp);
        Retry(() => File.Move(temp, targetPath, overwrite: true));
    }

    /// <summary>Only plain lap folder names are acted upon (no paths, no traversal).</summary>
    public static bool IsSafeLapFolderName(string name) =>
        !string.IsNullOrWhiteSpace(name)
        && name.StartsWith("Lap_", StringComparison.OrdinalIgnoreCase)
        && name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0
        && !name.Contains("..", StringComparison.Ordinal);

    private static void Restore(string runPath, string backup, IReadOnlyList<string> moved, IReadOnlyList<string> created, bool hadRunLapHeader)
    {
        var errors = new List<string>();
        foreach (var name in created)
        {
            var path = Path.Combine(runPath, name);
            try
            {
                if (Directory.Exists(path))
                    Retry(() => Directory.Delete(path, recursive: true));
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                errors.Add($"{name}: {ex.Message}");
            }
        }

        foreach (var name in moved)
        {
            try
            {
                var target = Path.Combine(runPath, name);
                if (!Directory.Exists(target))
                    Retry(() => Directory.Move(Path.Combine(backup, name), target));
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                errors.Add($"{name}: {ex.Message}");
            }
        }

        var savedHeader = Path.Combine(backup, RunLapHeaderFile);
        if (hadRunLapHeader && File.Exists(savedHeader))
            File.Copy(savedHeader, Path.Combine(runPath, RunLapHeaderFile), overwrite: true);

        if (errors.Count > 0)
            throw new IOException($"Restoring the original laps was incomplete ({string.Join("; ", errors)}). Originals remain in {backup}.");
    }

    /// <summary>
    /// Runs a file-system operation, retrying briefly when another process (virus scanner, indexer, OneDrive) holds a freshly
    /// written file for a moment. Persistent errors are rethrown after ~1.5 s.
    /// </summary>
    internal static void Retry(Action operation)
    {
        for (var attempt = 0; ; attempt++)
        {
            try
            {
                operation();
                return;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException && attempt < 4 && ex is not DirectoryNotFoundException and not FileNotFoundException)
            {
                Thread.Sleep(100 << attempt);
            }
        }
    }

    private static string CreateBackupFolder(string runPath)
    {
        var root = Path.Combine(runPath, BackupFolderName);
        var stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
        var folder = Path.Combine(root, stamp);
        for (var i = 1; Directory.Exists(folder); i++)
            folder = Path.Combine(root, $"{stamp}-{i}");
        Directory.CreateDirectory(folder);
        return folder;
    }

    private static void CopyMissingLapFiles(string sourceLap, string targetLap)
    {
        if (!Directory.Exists(sourceLap))
            return;
        foreach (var file in Directory.GetFiles(sourceLap, "*.xml", SearchOption.TopDirectoryOnly))
        {
            var target = Path.Combine(targetLap, Path.GetFileName(file));
            if (!File.Exists(target))
                File.Copy(file, target);
        }
    }

    private static void WriteManifest(string backup, SplitManifest manifest) =>
        File.WriteAllText(Path.Combine(backup, ManifestFileName), JsonSerializer.Serialize(manifest, JsonOptions));

    private static string ReadAcquisition(string lapFolder)
    {
        try
        {
            var file = Path.Combine(lapFolder, LapHeaderFile);
            return File.Exists(file) ? XDocument.Load(file).Root?.Element("Abs")?.Value.Trim() ?? string.Empty : string.Empty;
        }
        catch (Exception ex) when (ex is IOException or System.Xml.XmlException or UnauthorizedAccessException)
        {
            return string.Empty;
        }
    }

    private static string FirstSegment(string? path) =>
        (path ?? string.Empty).Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;

    /// <summary>Shifts a WinTAX start time (wall clock and/or unix seconds + nanoseconds) by <paramref name="delta"/> seconds.</summary>
    private static bool TryShift(string? wallText, string? secondsText, string? nanosText, double delta,
        out DateTime wall, out long seconds, out long nanos)
    {
        wall = default;
        seconds = 0;
        nanos = 0;
        var haveWall = DateTime.TryParseExact(wallText?.Trim(), WallClockFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var w);
        var haveUnix = long.TryParse(secondsText?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var s);
        if (!haveUnix && !haveWall)
            return false;

        var deltaNs = (long)Math.Round(delta * 1e9);
        if (haveWall)
            wall = w.AddTicks(deltaNs / 100);
        if (haveUnix)
        {
            long.TryParse(nanosText?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var ns);
            var total = s * 1_000_000_000L + ns + deltaNs;
            seconds = Math.DivRem(total, 1_000_000_000L, out nanos);
        }
        else
        {
            seconds = new DateTimeOffset(wall, TimeSpan.Zero).ToUnixTimeSeconds();
            nanos = wall.Ticks % TimeSpan.TicksPerSecond * 100;
        }

        if (!haveWall)
            wall = DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime.AddTicks(nanos / 100);
        return true;
    }

    private static bool TryParseClock(string? text, out TimeSpan clock) =>
        TimeSpan.TryParseExact(text?.Trim(), @"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture, out clock);

    private static void Set(XElement? parent, string name, string value)
    {
        if (parent?.Element(name) is { } element)
            element.Value = value;
    }

    private static string? GetInfo(XElement? infos, string name) =>
        infos?.Elements("Info").FirstOrDefault(i => (string?)i.Element("Name") == name)?.Element("Data")?.Value;

    private static void SetInfo(XElement? infos, string name, string value)
    {
        if (infos?.Elements("Info").FirstOrDefault(i => (string?)i.Element("Name") == name)?.Element("Data") is { } data)
            data.Value = value;
    }
}
