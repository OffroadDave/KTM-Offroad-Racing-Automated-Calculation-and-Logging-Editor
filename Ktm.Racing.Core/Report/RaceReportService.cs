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
    private readonly VchSettingsReader _vchSettingsReader = new();

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
            .ThenBy(r => r.RunName, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var first = ordered[0];
        if (string.IsNullOrWhiteSpace(first.Driver))
            warnings.Add("Driver ist leer – ORACLE wurde evtl. noch nicht ausgeführt.");

        var channelNames = inputs.Select(i => i.Channel).ToList();
        IReadOnlyDictionary<string, double?> aggregates = ordered.Count == 1
            ? ReadSingle(first, vchFiles, channelNames, progress)
            : ReadMerged(ordered, vchFiles, channelNames, progress);

        // Anzeige-Einstellungen (Einheit, Nachkommastellen, OutFormat) einmalig aus den VCH-Dateien lesen.
        var vchSettings = _vchSettingsReader.Read(vchFiles);

        var values = new List<RaceReportValue>(inputs.Count);
        foreach (var input in inputs)
        {
            aggregates.TryGetValue(input.Channel, out var value);
            if (value is null) warnings.Add($"Kein Wert für Kanal: {input.Channel}");

            // Einheit und Nachkommastellen gemäß VCH-Einstellung (Fallback: Unit aus der Input-Excel).
            vchSettings.TryGetValue(input.Channel, out var settings);
            var unit = !string.IsNullOrEmpty(settings?.Unit) ? settings!.Unit : input.Unit;
            values.Add(new RaceReportValue(input.Alias, unit, ApplyVchFormat(value, settings), settings?.Decimals));
        }

        return new RaceReportResult
        {
            Track = first.Track,
            Session = first.Session,
            Device = first.Device,
            Driver = first.Driver,
            SessionDate = first.AcquisitionTime, // echtes AcquireTime aus RunLapHeader.xml (kopiersicher).
            Values = values,
            RunNames = ordered.Select(r => r.RunName).ToList(),
            Warnings = warnings,
        };
    }

    // Rundet den Wert auf die in der VCH definierten Nachkommastellen (nur bei numerischen Ausgabeformaten).
    private static double? ApplyVchFormat(double? value, VchChannelSettings? settings)
    {
        if (value is null || settings?.Decimals is not int decimals) return value;
        if (IsHexLike(settings.OutFormat)) return value; // Hex/Bin/Ascii nicht runden.
        return Math.Round(value.Value, Math.Clamp(decimals, 0, 15), MidpointRounding.AwayFromZero);
    }

    private static bool IsHexLike(string outFormat) =>
        outFormat.Equals("Hex", StringComparison.OrdinalIgnoreCase)
        || outFormat.Equals("Bin", StringComparison.OrdinalIgnoreCase)
        || outFormat.Equals("Ascii", StringComparison.OrdinalIgnoreCase);

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
