using Ktm.Racing.Core;
using Xunit;

namespace Ktm.Racing.Tests;

public class RunDiscoveryLapInfoTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), $"lapinfo_{Guid.NewGuid():N}");

    public RunDiscoveryLapInfoTests() => Directory.CreateDirectory(_dir);

    private string WriteRunLapHeader(params long[] lapTimes)
    {
        var run = Path.Combine(_dir, $"Run_{Guid.NewGuid():N}");
        Directory.CreateDirectory(run);
        var laps = string.Concat(lapTimes.Select(t =>
            $"<Lap><Infos><Info><Name>LapTime</Name><Data>{t}</Data><Infos/></Info></Infos></Lap>"));
        File.WriteAllText(Path.Combine(run, "RunLapHeader.xml"),
            $"<RunLapHeader><Run><LapArray>{laps}</LapArray></Run></RunLapHeader>");
        return run;
    }

    [Fact]
    public void ReadLapInfo_SumsLapTimes_AsDuration()
    {
        var run = WriteRunLapHeader(32190, 1273800); // 3.219s + 127.38s = 130.599s

        var (lapCount, duration, _) = RunDiscoveryService.ReadLapInfo(run);

        Assert.Equal(2, lapCount);
        Assert.NotNull(duration);
        Assert.Equal(130.599, duration!.Value.TotalSeconds, 3);
    }

    [Fact]
    public void ReadLapInfo_NoLapTimeFields_DurationNull()
    {
        var run = Path.Combine(_dir, "Run_NoTime");
        Directory.CreateDirectory(run);
        File.WriteAllText(Path.Combine(run, "RunLapHeader.xml"),
            "<RunLapHeader><Run><LapArray><Lap><Infos/></Lap></LapArray></Run></RunLapHeader>");

        var (lapCount, duration, _) = RunDiscoveryService.ReadLapInfo(run);

        Assert.Equal(1, lapCount);
        Assert.Null(duration);
    }

    [Fact]
    public void ReadLapInfo_MissingFile_ReturnsZeroAndNull()
    {
        var (lapCount, duration, acquisitionTime) = RunDiscoveryService.ReadLapInfo(Path.Combine(_dir, "nope"));
        Assert.Equal(0, lapCount);
        Assert.Null(duration);
        Assert.Null(acquisitionTime);
    }

    [Fact]
    public void ReadLapInfo_ParsesEarliestAcquireTime()
    {
        var run = Path.Combine(_dir, "Run_Acq");
        Directory.CreateDirectory(run);
        File.WriteAllText(Path.Combine(run, "RunLapHeader.xml"),
            "<RunLapHeader><Run><LapArray>" +
            "<Lap><Infos><Info><Name>AcquireTime</Name><Data>31.05.2026 11:25:45</Data></Info></Infos></Lap>" +
            "<Lap><Infos><Info><Name>AcquireTime</Name><Data>31.05.2026 11:20:00</Data></Info></Infos></Lap>" +
            "</LapArray></Run></RunLapHeader>");

        var (_, _, acquisitionTime) = RunDiscoveryService.ReadLapInfo(run);

        Assert.Equal(new DateTime(2026, 5, 31, 11, 20, 0), acquisitionTime);
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch { }
    }
}
