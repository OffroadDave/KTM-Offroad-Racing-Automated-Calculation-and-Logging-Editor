using System.Xml.Linq;
using KtmDataHandler.Marelli.Wintax;

namespace Ktm.Racing.Core;

/// <summary>
/// Findet Runs unterhalb eines Session-/Track-Ordners und liest die Anzeige-Metadaten
/// (Track, Session, Device, Driver, Lap-Anzahl, Startzeit) direkt aus den WinTAX4-XML-Dateien.
///
/// Run-Discovery erfolgt über <see cref="IMarelliDataBrowse.FindRuns"/>; die Metadaten werden
/// bewusst leichtgewichtig aus Session.xml/Car.xml/RunLapHeader.xml gelesen (kein Laden der
/// Telemetrie nötig). Ergebnis ist chronologisch nach echter Startzeit sortiert.
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
    /// chronologisch sortiert nach Startzeit (Fallback: Ordnername).
    /// </summary>
    public IReadOnlyList<DiscoveredRun> Discover(
        string folder, IProgress<string>? progress = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(folder)) throw new ArgumentException("Ordnerpfad erforderlich.", nameof(folder));
        if (!Directory.Exists(folder)) throw new DirectoryNotFoundException($"Ordner nicht gefunden: {folder}");

        progress?.Report($"Suche Runs in: {folder}");
        var runPaths = _browse.FindRuns(folder) ?? Enumerable.Empty<string>();

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
            .ThenBy(r => r.StartTime ?? DateTime.MaxValue)
            .ThenBy(r => r.RunName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static DiscoveredRun ReadRun(string runPath)
    {
        var (track, session) = ReadSession(runPath);
        var (device, driver) = ReadCar(runPath);
        var (lapCount, startTime) = ReadLapHeader(runPath);

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
            StartTime = startTime,
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

    private static (int LapCount, DateTime? StartTime) ReadLapHeader(string runPath)
    {
        var path = Path.Combine(runPath, "RunLapHeader.xml");
        if (!File.Exists(path)) return (0, null);

        var root = XDocument.Load(path).Root;
        var lapCount = root?.Element("Run")?.Element("LapArray")?.Elements("Lap").Count() ?? 0;

        DateTime? startTime = null;
        var modT = root?.Element("Inf")?.Element("ModT")?.Value;
        if (long.TryParse(modT, out var unixSeconds) && unixSeconds > 0)
            startTime = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).LocalDateTime;

        return (lapCount, startTime);
    }
}
