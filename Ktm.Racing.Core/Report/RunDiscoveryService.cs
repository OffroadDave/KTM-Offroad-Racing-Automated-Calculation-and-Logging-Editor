using System.Globalization;
using System.Xml.Linq;
using KtmDataHandler.Marelli.Wintax;

namespace Ktm.Racing.Core;

/// <summary>
/// Findet Runs unterhalb eines Session-/Track-Ordners und liest die Anzeige-Metadaten
/// (Track, Session, Device, Driver, Lap-Anzahl, Akquisitions-Reihenfolge) direkt aus den WinTAX4-XML-Dateien.
///
/// Run-Discovery erfolgt über <see cref="IMarelliDataBrowse.FindRuns"/>; die Metadaten werden
/// bewusst leichtgewichtig aus Session.xml/Car.xml/RunLapHeader.xml gelesen (kein Laden der
/// Telemetrie nötig). Ergebnis ist chronologisch nach Akquisitions-Reihenfolge (Lap-Zähler) sortiert.
/// </summary>
public sealed class RunDiscoveryService
{
    private readonly IMarelliDataBrowse _browse;

    /// <summary>Standard-Konstruktor: erzeugt eine eigene MarelliDataBrowse-Instanz.</summary>
    public RunDiscoveryService()
    {
        KtmRuntime.EnsureInitialized();
        _browse = new MarelliDataBrowse();
    }

    /// <summary>Test-Konstruktor: erlaubt das Einschleusen einer IMarelliDataBrowse-Instanz.</summary>
    internal RunDiscoveryService(IMarelliDataBrowse browse)
    {
        KtmRuntime.EnsureInitialized();
        _browse = browse;
    }

    /// <summary>
    /// Ermittelt alle Runs unter <paramref name="folder"/> (Track- oder Session-Ordner),
    /// chronologisch sortiert nach Akquisitions-Reihenfolge (Fallback: Ordnername).
    /// </summary>
    public IReadOnlyList<DiscoveredRun> Discover(
        string folder, IProgress<string>? progress = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(folder)) throw new ArgumentException("Ordnerpfad erforderlich.", nameof(folder));
        if (!Directory.Exists(folder)) throw new DirectoryNotFoundException($"Ordner nicht gefunden: {folder}");

        progress?.Report($"Suche Runs in: {folder}");

        // Primär: Marelli-Run-Erkennung.
        var runPaths = new List<string>(_browse.FindRuns(folder) ?? Enumerable.Empty<string>());
        var seen = new HashSet<string>(runPaths.Select(NormalizePath), StringComparer.OrdinalIgnoreCase);

        // Fallback: reprozessierte Runs (z.B. Lap_..._Def/App mit cableData.old) erkennt FindRuns nicht.
        // Zusätzlich Run_*-Ordner aufnehmen, die die Run-XMLs UND irgendwo darunter eine cableData.ztx haben.
        int recovered = 0;
        foreach (var runPath in ScanRunFolders(folder))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (seen.Add(NormalizePath(runPath)))
            {
                runPaths.Add(runPath);
                recovered++;
            }
        }
        if (recovered > 0)
            progress?.Report($"{recovered} zusätzliche(r) Run(s) über Fallback-Scan erkannt.");

        var runs = new List<DiscoveredRun>();
        foreach (var runPath in runPaths)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                runs.Add(ReadRun(runPath));
            }
            catch (Exception ex)
            {
                progress?.Report($"WARN: Run übersprungen ({Path.GetFileName(runPath)}): {ex.Message}");
            }
        }

        progress?.Report($"{runs.Count} Run(s) gefunden.");
        return runs
            .OrderBy(r => r.AcquisitionOrder ?? long.MaxValue)
            .ThenBy(r => r.RunName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// Fallback zur Marelli-Run-Erkennung: findet Run_*-Ordner, die die Run-XMLs
    /// (Session.xml, Car.xml, RunLapHeader.xml) UND irgendwo darunter eine cableData.ztx besitzen.
    /// Fängt reprozessierte Runs ab (z.B. Lap_..._Def/App), die <see cref="IMarelliDataBrowse.FindRuns"/> auslässt.
    /// </summary>
    internal static IReadOnlyList<string> ScanRunFolders(string folder)
    {
        List<string> candidates;
        try
        {
            candidates = Directory.GetDirectories(folder, "Run_*", SearchOption.AllDirectories).ToList();
        }
        catch
        {
            return Array.Empty<string>(); // Best-effort: der Primärpfad (FindRuns) darf nicht scheitern.
        }

        return candidates.Where(dir => HasRunXml(dir) && ContainsCableData(dir)).ToList();
    }

    private static bool HasRunXml(string runPath) =>
        File.Exists(Path.Combine(runPath, "RunLapHeader.xml"))
        && File.Exists(Path.Combine(runPath, "Session.xml"))
        && File.Exists(Path.Combine(runPath, "Car.xml"));

    private static bool ContainsCableData(string runPath)
    {
        try { return Directory.EnumerateFiles(runPath, "cableData.ztx", SearchOption.AllDirectories).Any(); }
        catch { return false; }
    }

    private static string NormalizePath(string path)
    {
        try { return Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar); }
        catch { return path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar); }
    }

    private static DiscoveredRun ReadRun(string runPath)
    {
        var (track, session) = ReadSession(runPath);
        var (device, driver) = ReadCar(runPath);
        var (lapCount, duration, acquisitionTime) = ReadLapInfo(runPath);

        if (lapCount == 0 && Directory.Exists(runPath))
            lapCount = Directory.GetDirectories(runPath, "Lap_*", SearchOption.TopDirectoryOnly).Length;

        return new DiscoveredRun
        {
            RunPath = runPath,
            RunName = new DirectoryInfo(runPath).Name,
            Track = track,
            Session = session,
            Device = device,
            Driver = driver,
            LapCount = lapCount,
            Duration = duration,
            AcquisitionTime = acquisitionTime,
            AcquisitionOrder = ReadAcquisitionOrder(runPath),
        };
    }

    /// <summary>Kleinster Lap-Ordner-Zähler (Lap_00001101_00418 → 1101) als monotone Akquisitions-Reihenfolge.</summary>
    private static long? ReadAcquisitionOrder(string runPath)
    {
        if (!Directory.Exists(runPath)) return null;
        long? min = null;
        foreach (var dir in Directory.GetDirectories(runPath, "Lap_*", SearchOption.TopDirectoryOnly))
        {
            var parts = Path.GetFileName(dir).Split('_');
            if (parts.Length >= 2 && long.TryParse(parts[1], out var counter))
                min = min is null ? counter : Math.Min(min.Value, counter);
        }
        return min;
    }

    private static (string Track, string Session) ReadSession(string runPath)
    {
        var path = Path.Combine(runPath, "Session.xml");
        if (!File.Exists(path)) return ("", "");
        var root = XDocument.Load(path).Root;
        return (root?.Element("Track")?.Value?.Trim() ?? "", root?.Element("Name")?.Value?.Trim() ?? "");
    }

    private static (string Device, string Driver) ReadCar(string runPath)
    {
        var path = Path.Combine(runPath, "Car.xml");
        if (!File.Exists(path)) return ("", "");
        var root = XDocument.Load(path).Root;
        return (root?.Element("Name")?.Value?.Trim() ?? "", root?.Element("DriverName")?.Value?.Trim() ?? "");
    }

    private static readonly string[] AcquireTimeFormats =
        { "dd.MM.yyyy HH:mm:ss", "d.M.yyyy H:mm:ss", "dd.MM.yyyy HH:mm" };

    /// <summary>
    /// Liest aus RunLapHeader.xml die Lap-Anzahl, die Gesamtdauer (Summe der Lap-Zeiten) und den
    /// echten Aufnahmezeitpunkt (frühestes AcquireTime). LapTime ist in 1/10 ms (LapTime/10000 = Sekunden).
    /// </summary>
    internal static (int LapCount, TimeSpan? Duration, DateTime? AcquisitionTime) ReadLapInfo(string runPath)
    {
        var path = Path.Combine(runPath, "RunLapHeader.xml");
        if (!File.Exists(path)) return (0, null, null);

        var lapArray = XDocument.Load(path).Root?.Element("Run")?.Element("LapArray");
        if (lapArray is null) return (0, null, null);

        var laps = lapArray.Elements("Lap").ToList();
        double totalSeconds = 0;
        bool anyLapTime = false;
        DateTime? acquisitionTime = null;

        foreach (var lap in laps)
        {
            var infos = (lap.Element("Infos")?.Elements("Info") ?? Enumerable.Empty<XElement>()).ToList();
            string? Info(string name) => infos.FirstOrDefault(i => (string?)i.Element("Name") == name)?.Element("Data")?.Value;

            if (long.TryParse(Info("LapTime"), out var lapTime))
            {
                totalSeconds += lapTime / 10000.0;
                anyLapTime = true;
            }

            if (DateTime.TryParseExact(Info("AcquireTime"), AcquireTimeFormats,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
                && (acquisitionTime is null || dt < acquisitionTime))
            {
                acquisitionTime = dt;
            }
        }

        return (laps.Count, anyLapTime ? TimeSpan.FromSeconds(totalSeconds) : null, acquisitionTime);
    }
}
