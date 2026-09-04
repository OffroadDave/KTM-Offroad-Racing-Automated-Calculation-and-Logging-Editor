using Ktm.Racing.Core;
using Xunit;

namespace Ktm.Racing.Tests;

public class ChannelReductionTests
{
    private static RunAggregateResult Run(double durationSeconds, params (string Name, double? Value)[] values)
        => new(values.ToDictionary(v => v.Name, v => v.Value), durationSeconds);

    [Theory]
    [InlineData("TOil_max", (int)ReduceOp.Max)]
    [InlineData("nEngine_max", (int)ReduceOp.Max)]
    [InlineData("VolFuelUsedLoaded_max", (int)ReduceOp.Max)]
    [InlineData("TOil_min", (int)ReduceOp.Min)]
    [InlineData("TOil_avg", (int)ReduceOp.WeightedAvg)]
    [InlineData("TEng_ActiveLC_avg", (int)ReduceOp.WeightedAvg)]
    [InlineData("Time_TOil175", (int)ReduceOp.Sum)]
    [InlineData("Time_TOil175_max", (int)ReduceOp.Sum)]
    [InlineData("OperatingHours", (int)ReduceOp.Sum)]
    public void OpFor_MapsTargetChannels(string name, int expected)
        => Assert.Equal((ReduceOp)expected, ChannelReduction.OpFor(name));

    [Fact]
    public void Reduce_Max_TakesLargestAcrossRuns()
    {
        var runs = new[] { Run(10, ("TOil_max", 163.0)), Run(20, ("TOil_max", 166.0)) };
        Assert.Equal(166.0, ChannelReduction.Reduce(new[] { "TOil_max" }, runs)["TOil_max"]);
    }

    [Fact]
    public void Reduce_Min_TakesSmallestAcrossRuns()
    {
        var runs = new[] { Run(10, ("TOil_min", 155.0)), Run(20, ("TOil_min", 150.0)) };
        Assert.Equal(150.0, ChannelReduction.Reduce(new[] { "TOil_min" }, runs)["TOil_min"]);
    }

    [Fact]
    public void Reduce_Sum_ForTimeAndOperatingHours()
    {
        var runs = new[]
        {
            Run(10, ("OperatingHours", 0.2), ("Time_TEng140", 0.4)),
            Run(10, ("OperatingHours", 0.5), ("Time_TEng140", 0.6)),
        };
        var result = ChannelReduction.Reduce(new[] { "OperatingHours", "Time_TEng140" }, runs);
        Assert.Equal(0.7, result["OperatingHours"]!.Value, 3);
        Assert.Equal(1.0, result["Time_TEng140"]!.Value, 3);
    }

    [Fact]
    public void Reduce_WeightedAvg_WeightsByDuration()
    {
        // 100 °C over 10 s and 200 °C over 30 s → (100*10 + 200*30) / 40 = 175
        var runs = new[] { Run(10, ("TOil_avg", 100.0)), Run(30, ("TOil_avg", 200.0)) };
        Assert.Equal(175.0, ChannelReduction.Reduce(new[] { "TOil_avg" }, runs)["TOil_avg"]!.Value, 3);
    }

    [Fact]
    public void Reduce_SkipsNullValues()
    {
        var runs = new[] { Run(10, ("TOil_max", (double?)null)), Run(20, ("TOil_max", 163.0)) };
        Assert.Equal(163.0, ChannelReduction.Reduce(new[] { "TOil_max" }, runs)["TOil_max"]);
    }

    [Fact]
    public void Reduce_AllNull_ReturnsNull()
    {
        var runs = new[] { Run(10, ("TEng_ActiveLC_avg", (double?)null)), Run(20, ("TEng_ActiveLC_avg", (double?)null)) };
        Assert.Null(ChannelReduction.Reduce(new[] { "TEng_ActiveLC_avg" }, runs)["TEng_ActiveLC_avg"]);
    }
}
