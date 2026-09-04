namespace Ktm.Racing.Core;

/// <summary>
/// Orchestriert den Race Report: Input-Kanäle lesen → (bei mehreren Runs) mergen → VCH-Aggregate lesen
/// → Ergebnis mit Metadaten zusammenbauen. Die Excel-Ausgabe erfolgt separat über <see cref="RaceReportExcelWriter"/>.
/// </summary>
public sealed class RaceReportService
{
    private readonly RaceReportOptions _options;
    private readonly RaceReportInputReader _inputReader = new();
    private readonly AggregateChannelReader _aggregateReader = new();

    public RaceReportService(RaceReportOptions? options = null) => _options = options ?? new RaceReportOptions();

    public RaceReportResult Generate(IReadOnlyList<DiscoveredRun> selectedRuns, IProgress<string>? progress = null)
    {
        if (selectedRuns is null || selectedRuns.Count == 0)
            throw new ArgumentException("Mindestens ein Run muss ausgewählt sein.", nameof(selectedRuns));

        var warnings = new List<string>();

        progress?.Report("Lese Input-Kanäle …");
        var inputs = _inputReader.Read(_options.InputExcelPath, _options.InputSheetName);
        if (inputs.Count == 0)
            throw new InvalidDataException("Input-Excel enthält keine aktiven Kanäle (LOAD=1 & CALC=1).");

        var vchFiles = _options.EffectiveVchFiles();
        if (vchFiles.Count == 0)
            throw new FileNotFoundException("Keine VCH-Bibliotheken gefunden (siehe RaceReportOptions).");

        // Chronologisch nach Akquisitions-Reihenfolge (Lap-Zähler) sortieren; Metadaten aus dem ersten Run.
        var ordered = selectedRuns
            .OrderBy(r => r.AcquisitionOrder ?? long.MaxValue)
            .ThenBy(r => r.StartTime ?? DateTime.MaxValue)
            .ThenBy(r => r.RunName, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var first = ordered[0];
        if (string.IsNullOrWhiteSpace(first.Driver))
            warnings.Add("Driver ist leer – ORACLE wurde evtl. noch nicht ausgeführt.");

        var channelNames = inputs.Select(i => i.Channel).ToList();
        IReadOnlyDictionary<string, double?> aggregates = ordered.Count == 1
            ? ReadSingle(first, vchFiles, channelNames, progress)
            : ReadMerged(ordered, vchFiles, channelNames, progress);

        var values = new List<RaceReportValue>(inputs.Count);
        foreach (var input in inputs)
        {
            aggregates.TryGetValue(input.Channel, out var value);
            if (value is null) warnings.Add($"Kein Wert für Kanal: {input.Channel}");
            values.Add(new RaceReportValue(input.Alias, input.Unit, value));
        }

        return new RaceReportResult
        {
            Track = first.Track,
            Session = first.Session,
            Device = first.Device,
            Driver = first.Driver,
            SessionDate = first.StartTime,
            Values = values,
            RunNames = ordered.Select(r => r.RunName).ToList(),
            Warnings = warnings,
        };
    }

    private IReadOnlyDictionary<string, double?> ReadSingle(
        DiscoveredRun run, List<string> vchFiles, List<string> channelNames, IProgress<string>? progress)
    {
        progress?.Report($"Lese Kanäle aus {run.RunName} …");
        return _aggregateReader.ReadAggregates(run.RunPath, vchFiles, _options.VchFolder, channelNames).Values;
    }

    // Mehrere Runs: pro Run die VCH-Aggregate lesen und je nach Aggregat-Typ zusammenführen.
    // (Der Merge-Export-Weg liefert der VCH-Engine nur die Template-Lap-Daten – daher hier Reduktion.)
    private IReadOnlyDictionary<string, double?> ReadMerged(
        IReadOnlyList<DiscoveredRun> runs, List<string> vchFiles, List<string> channelNames, IProgress<string>? progress)
    {
        var perRun = new List<RunAggregateResult>(runs.Count);
        foreach (var run in runs)
        {
            progress?.Report($"Lese Kanäle aus {run.RunName} …");
            perRun.Add(_aggregateReader.ReadAggregates(run.RunPath, vchFiles, _options.VchFolder, channelNames));
        }
        return ChannelReduction.Reduce(channelNames, perRun);
    }
}
