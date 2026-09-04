namespace Ktm.Racing.Core;

internal enum ReduceOp { Max, Min, Sum, WeightedAvg }

/// <summary>
/// Führt die pro Run gelesenen VCH-Aggregate über mehrere Runs zusammen.
/// Zuordnung anhand des VCH-Namens: _max→Max, _min→Min, _avg→dauergewichtetes Mittel,
/// Time_* und OperatingHours→Summe. Deckt die definierten Ziel-Kanäle exakt ab.
/// </summary>
internal static class ChannelReduction
{
    public static ReduceOp OpFor(string name)
    {
        if (name.StartsWith("Time_", StringComparison.OrdinalIgnoreCase)
            || name.Equals("OperatingHours", StringComparison.OrdinalIgnoreCase)) return ReduceOp.Sum;
        if (name.EndsWith("_min", StringComparison.OrdinalIgnoreCase)) return ReduceOp.Min;
        if (name.EndsWith("_avg", StringComparison.OrdinalIgnoreCase)) return ReduceOp.WeightedAvg;
        return ReduceOp.Max;
    }

    public static IReadOnlyDictionary<string, double?> Reduce(
        IReadOnlyList<string> channelNames, IReadOnlyList<RunAggregateResult> perRun)
    {
        var result = new Dictionary<string, double?>(StringComparer.Ordinal);
        foreach (var name in channelNames)
        {
            var op = OpFor(name);
            bool any = false;
            double best = op == ReduceOp.Min ? double.MaxValue : double.MinValue;
            double sum = 0, weight = 0;

            foreach (var run in perRun)
            {
                if (!run.Values.TryGetValue(name, out var value) || value is null) continue;
                double v = value.Value;
                any = true;
                switch (op)
                {
                    case ReduceOp.Max: best = Math.Max(best, v); break;
                    case ReduceOp.Min: best = Math.Min(best, v); break;
                    case ReduceOp.Sum: sum += v; break;
                    case ReduceOp.WeightedAvg:
                        double w = run.DurationSeconds > 0 ? run.DurationSeconds : 1.0;
                        sum += v * w; weight += w; break;
                }
            }

            result[name] = !any ? null : op switch
            {
                ReduceOp.Max or ReduceOp.Min => best,
                ReduceOp.Sum => sum,
                ReduceOp.WeightedAvg => weight > 0 ? sum / weight : null,
                _ => null,
            };
        }
        return result;
    }
}
