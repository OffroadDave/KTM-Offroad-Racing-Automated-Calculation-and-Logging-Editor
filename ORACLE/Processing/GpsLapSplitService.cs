using Ktm.Racing.Domain.Geo;
using Ktm.Racing.Domain.Telemetry;
using KtmDataHandler.Marelli.Wintax;
using KtmDataHandler.Models.Channels;

namespace Ktm.Racing.Oracle;

/// <summary>Outcome category of a GPS lap split on a single run.</summary>
public enum GpsLapSplitOutcomeStatus
{
    /// <summary>The run was re-split into laps and written.</summary>
    Split,

    /// <summary>Nothing was written (no GPS, no finish line pass, or fewer than two laps); non-fatal.</summary>
    Skipped,

    /// <summary>An unexpected error occurred.</summary>
    Failed,
}

/// <summary>The result of splitting one run by GPS.</summary>
public sealed class GpsLapSplitOutcome
{
    /// <summary>Outcome category.</summary>
    public required GpsLapSplitOutcomeStatus Status { get; init; }

    /// <summary>Number of laps written (0 when skipped/failed).</summary>
    public int LapCount { get; init; }

    /// <summary>Number of per-channel write errors reported by the write API.</summary>
    public int WriteErrors { get; init; }

    /// <summary>Human-readable summary.</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>Creates a <see cref="GpsLapSplitOutcomeStatus.Split"/> outcome.</summary>
    public static GpsLapSplitOutcome Split(int lapCount, int writeErrors, string message) =>
        new() { Status = GpsLapSplitOutcomeStatus.Split, LapCount = lapCount, WriteErrors = writeErrors, Message = message };

    /// <summary>Creates a non-fatal <see cref="GpsLapSplitOutcomeStatus.Skipped"/> outcome.</summary>
    public static GpsLapSplitOutcome Skipped(string message) =>
        new() { Status = GpsLapSplitOutcomeStatus.Skipped, Message = message };

    /// <summary>Creates a <see cref="GpsLapSplitOutcomeStatus.Failed"/> outcome.</summary>
    public static GpsLapSplitOutcome Failed(string message) =>
        new() { Status = GpsLapSplitOutcomeStatus.Failed, Message = message };
}

/// <summary>
/// Splits a WinTAX run into laps from a virtual GPS finish line, headlessly (no WinTAX GUI/COM):
/// it reads the run's channels, computes the lap-boundary times with the pure
/// <see cref="GpsLapSplitter"/>, and materializes the laps via
/// <c>IMarelliDataAccessWrite.SetChannelsSplitByLap</c>. Defensive and non-throwing per run.
/// </summary>
public sealed class GpsLapSplitService
{
    private readonly GpsLapSplitter _splitter = new();

    /// <summary>
    /// Re-splits a single run by GPS. The <paramref name="finishLine"/> is the KOMPASS track coordinate.
    /// Returns an outcome instead of throwing so a single bad run never aborts the pipeline.
    /// </summary>
    public GpsLapSplitOutcome SplitRun(string runPath, GeoCoordinate finishLine, GpsLapSplitOptions? options = null)
    {
        options ??= new GpsLapSplitOptions();

        var reader = KtmDataHandlerFactory.CreateReader();
        try
        {
            reader.SetDataLocation([runPath]);
            if (reader.LoadRun() < 0)
                return GpsLapSplitOutcome.Skipped("LoadRun fehlgeschlagen");

            var names = reader.GetChannelsNames() ?? [];
            var latName = Resolve(names, "Lat_dez", "lat");
            var lonName = Resolve(names, "Lon_dez", "lon");
            if (latName is null || lonName is null)
                return GpsLapSplitOutcome.Skipped("GPS-Kanäle (Lat_dez/Lon_dez) nicht verfügbar");

            // Alle Kanäle laden, um sie anschließend in Laps aufgeteilt zurückzuschreiben.
            IDataset dataset = reader.GetChannels(names);
            if (dataset is null || dataset.NumElements() == 0)
                return GpsLapSplitOutcome.Skipped("Keine Kanäle geladen");

            var latSignal = dataset.GetElement(latName)?.SignalData;
            var lonSignal = dataset.GetElement(lonName)?.SignalData;
            if (latSignal?.Data is not double[] lat || lonSignal?.Data is not double[] lon || latSignal.Time is not double[] time)
                return GpsLapSplitOutcome.Skipped("GPS-Daten nicht im erwarteten Format");

            var line = new GpsFinishLine { Latitude = finishLine.Latitude, Longitude = finishLine.Longitude };
            var split = _splitter.ComputeLapMarks(time, lat, lon, line, options);
            if (split.Status != GpsLapSplitStatus.Success || split.LapCount < 2)
                return GpsLapSplitOutcome.Skipped($"kein Split ({split.Status}: {split.Message})");

            var writer = KtmDataHandlerFactory.CreateWriter();
            try
            {
                writer.SetDataLocation([runPath]);
                var errors = writer.SetChannelsSplitByLap(dataset, [.. split.LapMarks]);
                var errorCount = errors?.Count(e => e.Item1 != 0) ?? 0;
                var marks = string.Join(", ", split.LapMarks.Select(m => m.ToString("F1")));
                return GpsLapSplitOutcome.Split(split.LapCount, errorCount,
                    $"{split.LapCount} Lap(s) geschrieben [Marks: {marks}], {errorCount} Schreibfehler.");
            }
            finally
            {
                writer.ReleaseObjects();
            }
        }
        catch (Exception ex)
        {
            return GpsLapSplitOutcome.Failed(ex.Message);
        }
        finally
        {
            reader.ReleaseObjects();
        }
    }

    private static string? Resolve(string[] names, string exact, string keyword) =>
        names.FirstOrDefault(n => n.Equals(exact, StringComparison.OrdinalIgnoreCase))
        ?? names.FirstOrDefault(n => n.Contains(keyword, StringComparison.OrdinalIgnoreCase));
}
