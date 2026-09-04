using KtmDataHandler.Models.Channels;

namespace Ktm.Racing.Core;

/// <summary>Aggregatwerte eines einzelnen Runs plus dessen Dauer (für gewichtete Mittelung beim Merge).</summary>
public sealed record RunAggregateResult(IReadOnlyDictionary<string, double?> Values, double DurationSeconds);

/// <summary>
/// Liest die (durch VCH berechneten) Aggregat-Kanäle aus einem geladenen Run.
///
/// Die getestete KtmDataHandler-Sequenz für VCH-Kanäle:
///   1. SetDataLocation([run], true)   (nur 1 Location wird unterstützt)
///   2. LoadRun()
///   3. SetMathLibrary(vch[], "", "", vchFolder)   (aktiviert die VCH)
///   4. LoadRun()   (Neuladen, damit die VCH-Kanäle berechnet werden)
///   5. GetChannels(namen) → IDataset; Aggregat = letzter gültiger Sample-Wert.
/// </summary>
public sealed class AggregateChannelReader
{
    public RunAggregateResult ReadAggregates(
        string runPath, IReadOnlyList<string> vchFiles, string vchFolder, IReadOnlyList<string> channelNames)
    {
        KtmRuntime.EnsureInitialized();

        var access = KtmDataHandlerFactory.CreateAccess();
        try
        {
            access.SetDataLocation(new[] { runPath }, true);
            access.LoadRun();
            access.SetMathLibrary(vchFiles.ToArray(), "", "", vchFolder);
            access.LoadRun();

            double lastTime = access.GetLastSampleTime();
            var names = channelNames.ToArray();
            IDataset dataset = access.GetChannels(names, 0.0, lastTime, names, false);

            var values = new Dictionary<string, double?>(StringComparer.Ordinal);
            foreach (var name in channelNames)
                values[name] = ExtractAggregate(dataset, name);

            return new RunAggregateResult(values, lastTime);
        }
        finally
        {
            // VCH wieder deaktivieren: sonst leakt der Zustand global (TelDataX) und blockiert späteres Schreiben (DHL1062).
            try { access.RemoveMathLibrary(); } catch { }
            try { access.ReleaseObjects(); } catch { }
        }
    }

    /// <summary>Letzter gültiger (nicht-NaN/Inf) Sample-Wert; VCH-Aggregate sind konstant bzw. monoton.</summary>
    private static double? ExtractAggregate(IDataset dataset, string name)
    {
        if (dataset.GetElement(name) is not ITimeseries timeseries) return null;
        if (timeseries.SignalData?.Data is not double[] data || data.Length == 0) return null;

        for (int i = data.Length - 1; i >= 0; i--)
        {
            var value = data[i];
            if (!double.IsNaN(value) && !double.IsInfinity(value)) return value;
        }
        return null;
    }
}
