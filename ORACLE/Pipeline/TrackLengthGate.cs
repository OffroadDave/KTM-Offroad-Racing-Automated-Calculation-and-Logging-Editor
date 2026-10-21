using System.Globalization;
using Ktm.Racing.Domain.Kompass;
using Ktm.Racing.Domain.MotoDb;
using Ktm.Racing.Domain.Telemetry;

namespace Ktm.Racing.Oracle;

/// <summary>Herkunft der von <see cref="TrackLengthGate"/> verwendeten Track-Länge.</summary>
public enum TrackLengthSource
{
    /// <summary>Keine Länge gefunden.</summary>
    None,

    /// <summary>Bestätigte Länge aus KOMPASS (Stammdaten).</summary>
    Kompass,

    /// <summary>Länge aus loadChannels.xlsx (Sheet CIRCUITS).</summary>
    MotoDb,

    /// <summary>Länge aus der .HED-Datei des Runs.</summary>
    Hed,

    /// <summary>Offener Vorschlag (bereits von einer anderen Verarbeitung gemessen, in KODE noch nicht bestätigt).</summary>
    Suggestion,

    /// <summary>Aus den GPS-Rundendistanzen dieser Verarbeitung geschätzt.</summary>
    Estimate,
}

/// <summary>Ergebnis der Track-Längen-Prüfung.</summary>
public enum TrackLengthGateStatus
{
    /// <summary>Eine eindeutige Länge ist bekannt (ggf. vorläufig).</summary>
    Ok,

    /// <summary>Keine eindeutige Länge – MotoDB darf nicht laufen.</summary>
    Missing,

    /// <summary>loadChannels.xlsx ist gesperrt (z.B. in Excel geöffnet) – erneut versuchen.</summary>
    Locked,
}

/// <summary>Die Entscheidung der Track-Längen-Prüfung.</summary>
/// <param name="Status">Ergebnis.</param>
/// <param name="LengthMeters">Die zu verwendende Länge (nur bei <see cref="TrackLengthGateStatus.Ok"/>).</param>
/// <param name="Source">Herkunft der Länge.</param>
/// <param name="Message">Zusammenfassung für Log/Status.</param>
/// <param name="Warnings">Hinweise, z.B. verworfene, nicht eindeutige Längen.</param>
public sealed record TrackLengthDecision(
    TrackLengthGateStatus Status,
    int? LengthMeters,
    TrackLengthSource Source,
    string Message,
    IReadOnlyList<string> Warnings)
{
    /// <summary>True, wenn die Länge noch nicht in KOMPASS bestätigt ist.</summary>
    public bool Provisional => Status == TrackLengthGateStatus.Ok && Source != TrackLengthSource.Kompass;

    /// <summary>Die Länge als Text für .HED/XML (leer, wenn keine).</summary>
    public string LengthText => LengthMeters?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
}

/// <summary>
/// Stellt vor MotoDB sicher, dass der Track eine <b>eindeutige</b> Länge hat. MotoDB vergleicht die
/// <c>trackLength</c> der .HED exakt mit der Spalte <c>Length</c> im Sheet CIRCUITS der loadChannels.xlsx;
/// zwei Strecken mit gleicher Länge führen zu einem Fehler.
///
/// <para><b>Priorität:</b> KOMPASS → CIRCUITS → .HED → offener Vorschlag → Schätzung aus den GPS-Rundendistanzen
/// (auf eine freie Länge verschoben). Jede Quelle wird nur verwendet, wenn kein anderer Track dieselbe
/// Länge nutzt (KOMPASS und CIRCUITS). Vorläufige Längen werden in CIRCUITS geschrieben und als Vorschlag
/// für KODE abgelegt, wo sie bestätigt werden.</para>
/// </summary>
public sealed class TrackLengthGate
{
    private readonly string _kompassPath;
    private readonly string _workbookPath;
    private readonly TrackLengthSettings _settings;
    private readonly LoadChannelsWriter _writer = new();
    private readonly List<string> _loadWarnings = [];
    private List<Track>? _kompassTracks;
    private List<CircuitEntry>? _circuits;

    /// <summary>Erstellt eine Prüfung für die angegebenen Dateien.</summary>
    public TrackLengthGate(string kompassPath, string workbookPath, TrackLengthSettings settings)
    {
        _kompassPath = kompassPath;
        _workbookPath = workbookPath;
        _settings = settings ?? new TrackLengthSettings();
    }

    /// <summary>Erstellt eine Prüfung aus der ORACLE-Konfiguration.</summary>
    public static TrackLengthGate FromConfig(OracleConfig config) =>
        new(config.DatabaseFilePath, config.MotoDbLoadChannelsPath, config.TrackLength);

    /// <summary>
    /// Ermittelt die Länge aus den vorhandenen Quellen (ohne Schätzung). Vor den Berechnungen aufrufen,
    /// damit .HED, XML und Renn-Erkennung bereits mit der richtigen Länge arbeiten.
    /// </summary>
    public TrackLengthDecision ResolveKnown(string trackName, string? hedLength)
    {
        var others = AllLengths();
        var warnings = new List<string>(_loadWarnings);
        if (string.IsNullOrWhiteSpace(trackName))
            return new(TrackLengthGateStatus.Missing, null, TrackLengthSource.None, "No track name in the metadata.", warnings);

        var kompassLengths = KompassTracks()
            .Where(t => TrackNames.Same(t.Name, trackName))
            .Select(t => TrackLengthRules.Parse(t.TrackLength))
            .OfType<int>()
            .Distinct()
            .ToList();
        if (kompassLengths.Count > 1)
            warnings.Add($"KOMPASS has different lengths for {trackName}: {string.Join(", ", kompassLengths)} m.");
        else if (kompassLengths.Count == 1 && Accept(trackName, kompassLengths[0], "KOMPASS", others, warnings))
            return Ok(kompassLengths[0], TrackLengthSource.Kompass, warnings);

        // In KODE abgelehnte Werte gelten nicht mehr – auch wenn ORACLE sie vorläufig in CIRCUITS/.HED geschrieben hat.
        var rejected = RejectedLengths(trackName);
        bool NotRejected(int length, string source)
        {
            if (!rejected.Contains(length))
                return true;
            warnings.Add($"{source} length {length} m was rejected in KODE – ignored.");
            return false;
        }

        var circuit = Circuits().FirstOrDefault(c => TrackNames.Same(c.Name, trackName) && c.LengthMeters is > 0);
        if (circuit?.LengthMeters is { } circuitLength && NotRejected(circuitLength, "loadChannels CIRCUITS")
            && Accept(trackName, circuitLength, "loadChannels CIRCUITS", others, warnings))
            return Ok(circuitLength, TrackLengthSource.MotoDb, warnings);

        if (TrackLengthRules.Parse(hedLength) is { } hed && NotRejected(hed, ".HED") && Accept(trackName, hed, ".HED", others, warnings))
            return Ok(hed, TrackLengthSource.Hed, warnings);

        var pending = Store()?.ReadPending()
            .Where(s => TrackNames.Same(s.TrackName, trackName) && !rejected.Contains(s.LengthMeters))
            .FirstOrDefault(s => Accept(trackName, s.LengthMeters, "suggestion", others, warnings));
        if (pending is not null)
            return Ok(pending.LengthMeters, TrackLengthSource.Suggestion, warnings);

        return new(TrackLengthGateStatus.Missing, null, TrackLengthSource.None,
            $"No unique length known for {trackName}.", warnings);
    }

    /// <summary>
    /// Schließt die Prüfung nach den Berechnungen ab: schätzt bei Bedarf die Länge aus den Rundendistanzen,
    /// schreibt sie in CIRCUITS und legt vorläufige Längen als Vorschlag für KODE ab.
    /// </summary>
    /// <param name="known">Ergebnis von <see cref="ResolveKnown"/>.</param>
    /// <param name="metadata">Run-Metadaten (Track, Land, Disziplin).</param>
    /// <param name="lapDistancesPerRun">GPS-Distanz je Runde, je Run.</param>
    /// <param name="origin">Herkunft für den Vorschlag (Run-/Rider-Ordner).</param>
    /// <param name="eventLapDistances">
    /// Optional: Rundendistanzen bereits verarbeiteter Runs desselben Events; werden nur genutzt, wenn die eigenen
    /// Runden für eine Schätzung nicht reichen (z.B. kurzer Run im KRAKEN-Einzel-Run-Modus).
    /// </param>
    public TrackLengthDecision Complete(TrackLengthDecision known, RunMetadata metadata,
        IEnumerable<IReadOnlyList<double>> lapDistancesPerRun, string origin,
        Func<IEnumerable<IReadOnlyList<double>>>? eventLapDistances = null)
    {
        ArgumentNullException.ThrowIfNull(known);
        ArgumentNullException.ThrowIfNull(metadata);
        var warnings = known.Warnings.ToList();
        var decision = known;
        TrackLengthEstimate? estimate = null;

        if (known.Status != TrackLengthGateStatus.Ok)
        {
            if (string.IsNullOrWhiteSpace(metadata.TrackName))
                return known;

            var own = lapDistancesPerRun.ToList();
            var estimation = TrackLengthEstimator.Estimate(own, _settings.ToOptions());
            if (!estimation.Success && eventLapDistances is not null)
            {
                var combined = own.Concat(eventLapDistances()).ToList();
                if (combined.Count > own.Count)
                {
                    var fromEvent = TrackLengthEstimator.Estimate(combined, _settings.ToOptions());
                    estimation = fromEvent.Success
                        ? fromEvent with { Message = $"{fromEvent.Message} incl. {combined.Count - own.Count} other processed run(s) of the event" }
                        : fromEvent with { Message = $"{estimation.Message} Event: {fromEvent.Message}" };
                }
            }

            if (!estimation.Success)
                return known with { Message = $"{known.Message} Estimate: {estimation.Message}" };

            estimate = estimation.Estimate!;
            if (RejectedLengths(metadata.TrackName).Contains(estimate.LengthMeters))
                return known with { Message = $"{known.Message} Measured {estimate.LengthMeters} m was rejected in KODE – enter the length in KODE." };

            var used = TrackLengthRules.LengthsUsedByOthers(metadata.TrackName, AllLengths());
            var length = TrackLengthRules.MakeUnique(estimate.LengthMeters, used);
            if (length != estimate.LengthMeters)
                warnings.Add($"Measured {estimate.LengthMeters} m is used by another track; using {length} m instead.");
            decision = new(TrackLengthGateStatus.Ok, length, TrackLengthSource.Estimate,
                $"{length} m measured ({estimation.Message}) – provisional, confirm in KODE.", warnings);
        }

        var finalLength = decision.LengthMeters!.Value;

        // MotoDB vergleicht exakt mit CIRCUITS: dort muss genau diese Länge stehen.
        if (File.Exists(_workbookPath)
            && Circuits().FirstOrDefault(c => TrackNames.Same(c.Name, metadata.TrackName))?.LengthMeters != finalLength)
        {
            var kompassTrack = KompassTracks().FirstOrDefault(t => TrackNames.Same(t.Name, metadata.TrackName));
            var country = kompassTrack?.Country ?? metadata.CountryName;
            var code = kompassTrack?.CountryCode ?? metadata.CountryCode;
            var result = WorkbookGuard.WriteWithRetry(
                () => _writer.SetCircuitLength(new LoadChannelsOptions { WorkbookPath = _workbookPath },
                    metadata.TrackName, finalLength, country, code?.Trim().Length == 3 ? code : null),
                TimeSpan.FromSeconds(Math.Max(0, _settings.LockWaitSeconds)));
            switch (result.Status)
            {
                case LoadChannelsSyncStatus.Locked:
                    return decision with { Status = TrackLengthGateStatus.Locked, Message = result.Message };
                case LoadChannelsSyncStatus.Failed:
                    warnings.Add($"CIRCUITS could not be updated: {result.Message}");
                    break;
                default:
                    _circuits = null;
                    // Nur Konflikte zu diesem Track melden; Konflikte anderer Tracks meldet sync-motodb.
                    var relevant = result.Warnings.Where(w => w.Contains(metadata.TrackName, StringComparison.OrdinalIgnoreCase)).ToList();
                    warnings.AddRange(relevant);
                    if (result.Warnings.Count > relevant.Count)
                        Console.WriteLine($"[trackLength] {result.Warnings.Count - relevant.Count} length conflict(s) of other tracks in CIRCUITS (details: ORACLE sync-motodb, KODE).");
                    break;
            }
        }

        if (decision.Source is TrackLengthSource.Hed or TrackLengthSource.Estimate)
            WriteSuggestion(metadata, decision, estimate, origin, warnings);

        return decision with { Warnings = warnings.Distinct().ToList() };
    }

    private void WriteSuggestion(RunMetadata metadata, TrackLengthDecision decision, TrackLengthEstimate? estimate,
        string origin, List<string> warnings)
    {
        var store = Store();
        if (store is null)
        {
            warnings.Add("No suggestions folder configured – the length was not sent to KODE.");
            return;
        }

        try
        {
            if (store.ReadPending().Any(s => TrackNames.Same(s.TrackName, metadata.TrackName) && s.LengthMeters == decision.LengthMeters))
                return;

            store.Write(new TrackLengthSuggestion
            {
                TrackName = metadata.TrackName.Trim(),
                CountryCode = metadata.CountryCode,
                Discipline = metadata.DisciplineName,
                LengthMeters = decision.LengthMeters!.Value,
                Source = decision.Source == TrackLengthSource.Estimate ? "ORACLE GPS" : "HED",
                MeasuredMedianMeters = estimate?.MedianMeters ?? 0,
                LapCount = estimate?.LapCount ?? 0,
                RelativeSpread = estimate?.RelativeSpread ?? 0,
                Origin = origin,
            });
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            warnings.Add($"Suggestion for KODE could not be written ({store.Directory}): {ex.Message}");
        }
    }

    /// <summary>Der Untergrund des Tracks laut KOMPASS (leer, wenn unbekannt).</summary>
    public string KompassSurface(string trackName) =>
        KompassTracks().FirstOrDefault(t => TrackNames.Same(t.Name, trackName) && !string.IsNullOrWhiteSpace(t.Surface))?.Surface.Trim()
        ?? string.Empty;

    /// <summary>Die in KODE abgelehnten Längen eines Tracks (leer ohne Vorschlagsordner).</summary>
    private IReadOnlySet<int> RejectedLengths(string trackName)
    {
        try
        {
            return Store()?.RejectedLengths(trackName) ?? new HashSet<int>();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return new HashSet<int>();
        }
    }

    private static TrackLengthDecision Ok(int length, TrackLengthSource source, List<string> warnings) =>
        new(TrackLengthGateStatus.Ok, length, source,
            source == TrackLengthSource.Kompass ? $"{length} m from KOMPASS." : $"{length} m from {source} – provisional, confirm in KODE.",
            warnings);

    private static bool Accept(string trackName, int length, string source, List<(string Name, int? LengthMeters)> others, List<string> warnings)
    {
        if (TrackLengthRules.IsUnique(trackName, length, others))
            return true;
        var users = others.Where(o => o.LengthMeters == length && !TrackNames.Same(o.Name, trackName))
            .Select(o => o.Name.Trim()).Distinct(StringComparer.OrdinalIgnoreCase);
        warnings.Add($"{source} length {length} m is also used by {string.Join(", ", users)} – MotoDB cannot tell them apart.");
        return false;
    }

    private List<(string Name, int? LengthMeters)> AllLengths() =>
        KompassTracks().Select(t => (t.Name, TrackLengthRules.Parse(t.TrackLength)))
            .Concat(Circuits().Select(c => (c.Name, c.LengthMeters)))
            .ToList();

    private TrackLengthSuggestionStore? Store()
    {
        var directory = _settings.ResolvedSuggestionsDirectory;
        return directory.Length == 0 ? null : new TrackLengthSuggestionStore(directory);
    }

    private List<Track> KompassTracks()
    {
        if (_kompassTracks is not null)
            return _kompassTracks;
        _kompassTracks = [];
        if (!File.Exists(_kompassPath))
        {
            _loadWarnings.Add($"KOMPASS not found: {_kompassPath}");
            return _kompassTracks;
        }

        try
        {
            _kompassTracks = KompassFlattener.FlattenTracks(new KompassRepository().Load(_kompassPath))
                .Select(r => r.Track)
                .Where(t => !string.IsNullOrWhiteSpace(t.Name))
                .ToList();
        }
        catch (Exception ex)
        {
            _loadWarnings.Add($"KOMPASS could not be read: {ex.Message}");
        }

        return _kompassTracks;
    }

    private List<CircuitEntry> Circuits()
    {
        if (_circuits is not null)
            return _circuits;
        try
        {
            _circuits = _writer.ReadCircuits(new LoadChannelsOptions { WorkbookPath = _workbookPath }).ToList();
        }
        catch (Exception ex)
        {
            _loadWarnings.Add($"loadChannels.xlsx could not be read: {ex.Message}");
            _circuits = [];
        }

        return _circuits;
    }
}
