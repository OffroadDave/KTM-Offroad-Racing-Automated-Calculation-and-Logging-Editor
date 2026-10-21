using System.Globalization;
using Ktm.Racing.Domain.Kompass;
using Ktm.Racing.Domain.MotoDb;
using Ktm.Racing.Domain.Telemetry;

namespace Ktm.Racing.Oracle;

/// <summary>Source of the track length used by <see cref="TrackLengthGate"/>.</summary>
public enum TrackLengthSource
{
    /// <summary>No length found.</summary>
    None,

    /// <summary>Confirmed length from KOMPASS (master data).</summary>
    Kompass,

    /// <summary>Length from loadChannels.xlsx (sheet CIRCUITS).</summary>
    MotoDb,

    /// <summary>Length from the run's .HED file.</summary>
    Hed,

    /// <summary>Open suggestion (already measured by another processing, not yet confirmed in KODE).</summary>
    Suggestion,

    /// <summary>Estimated from the GPS lap distances of this processing.</summary>
    Estimate,
}

/// <summary>Result of the track length check.</summary>
public enum TrackLengthGateStatus
{
    /// <summary>An unambiguous length is known (possibly provisional).</summary>
    Ok,

    /// <summary>No unambiguous length – MotoDB must not run.</summary>
    Missing,

    /// <summary>loadChannels.xlsx is locked (e.g. open in Excel) – try again.</summary>
    Locked,
}

/// <summary>The decision of the track length check.</summary>
/// <param name="Status">Result.</param>
/// <param name="LengthMeters">The length to use (only for <see cref="TrackLengthGateStatus.Ok"/>).</param>
/// <param name="Source">Source of the length.</param>
/// <param name="Message">Summary for log/status.</param>
/// <param name="Warnings">Notes, e.g. discarded, ambiguous lengths.</param>
public sealed record TrackLengthDecision(
    TrackLengthGateStatus Status,
    int? LengthMeters,
    TrackLengthSource Source,
    string Message,
    IReadOnlyList<string> Warnings)
{
    /// <summary>True if the length is not yet confirmed in KOMPASS.</summary>
    public bool Provisional => Status == TrackLengthGateStatus.Ok && Source != TrackLengthSource.Kompass;

    /// <summary>The length as text for .HED/XML (empty if none).</summary>
    public string LengthText => LengthMeters?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
}

/// <summary>
/// Ensures before MotoDB that the track has an <b>unambiguous</b> length. MotoDB compares the
/// <c>trackLength</c> of the .HED exactly with the column <c>Length</c> in the sheet CIRCUITS of loadChannels.xlsx;
/// two tracks with the same length lead to an error.
///
/// <para><b>Priority:</b> KOMPASS → CIRCUITS → .HED → open suggestion → estimate from the GPS lap distances
/// (shifted to a free length). Each source is only used if no other track uses the same
/// length (KOMPASS and CIRCUITS). Provisional lengths are written to CIRCUITS and stored as a suggestion
/// for KODE, where they are confirmed.</para>
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

    /// <summary>Creates a check for the specified files.</summary>
    public TrackLengthGate(string kompassPath, string workbookPath, TrackLengthSettings settings)
    {
        _kompassPath = kompassPath;
        _workbookPath = workbookPath;
        _settings = settings ?? new TrackLengthSettings();
    }

    /// <summary>Creates a check from the ORACLE configuration.</summary>
    public static TrackLengthGate FromConfig(OracleConfig config) =>
        new(config.DatabaseFilePath, config.MotoDbLoadChannelsPath, config.TrackLength);

    /// <summary>
    /// Determines the length from the existing sources (without estimate). Call before the calculations
    /// so that .HED, XML and race detection already work with the correct length.
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

        // Values rejected in KODE no longer apply – even if ORACLE has provisionally written them to CIRCUITS/.HED.
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
    /// Completes the check after the calculations: estimates the length from the lap distances if needed,
    /// writes it to CIRCUITS and stores provisional lengths as a suggestion for KODE.
    /// </summary>
    /// <param name="known">Result of <see cref="ResolveKnown"/>.</param>
    /// <param name="metadata">Run metadata (track, country, discipline).</param>
    /// <param name="lapDistancesPerRun">GPS distance per lap, per run.</param>
    /// <param name="origin">Origin for the suggestion (run/rider folder).</param>
    /// <param name="eventLapDistances">
    /// Optional: lap distances of already processed runs of the same event; only used if the own
    /// laps are not sufficient for an estimate (e.g. a short run in KRAKEN single-run mode).
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

        // MotoDB compares exactly with CIRCUITS: exactly this length must be stored there.
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
                    // Only report conflicts for this track; conflicts of other tracks are reported by sync-motodb.
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

    /// <summary>The track surface according to KOMPASS (empty if unknown).</summary>
    public string KompassSurface(string trackName) =>
        KompassTracks().FirstOrDefault(t => TrackNames.Same(t.Name, trackName) && !string.IsNullOrWhiteSpace(t.Surface))?.Surface.Trim()
        ?? string.Empty;

    /// <summary>The lengths of a track rejected in KODE (empty without a suggestions folder).</summary>
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
