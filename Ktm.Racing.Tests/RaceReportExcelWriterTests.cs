using ClosedXML.Excel;
using Ktm.Racing.Core;
using Xunit;

namespace Ktm.Racing.Tests;

public class RaceReportExcelWriterTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), $"rrout_{Guid.NewGuid():N}");

    private static RaceReportResult Sample() => new()
    {
        Track = "RT22_GER-Teutschenthal",
        Session = "R1",
        Device = "AA 01",
        Driver = "",
        SessionDate = new DateTime(2026, 5, 31),
        Values = new[]
        {
            new RaceReportValue("TOil_max", "\u00b0C", 163.0),
            new RaceReportValue("TEng_ActiveLC_avg", "\u00b0C", null),
        },
    };

    [Fact]
    public void WriteNew_CreatesFile_WithHeadersAndValues()
    {
        var path = new RaceReportExcelWriter().WriteNew(Sample(), _dir, autoOpen: false);

        Assert.True(File.Exists(path));
        Assert.Equal("RT22_GER-Teutschenthal_R1_AA 01.xlsx", Path.GetFileName(path));

        using var workbook = new XLWorkbook(path);
        var ws = workbook.Worksheets.First();
        Assert.Equal("Track", ws.Cell(1, 1).GetString());
        Assert.Equal("Ses.Date", ws.Cell(1, 5).GetString());
        Assert.Equal("TOil_max [\u00b0C]", ws.Cell(1, 6).GetString());
        Assert.Equal("31 - May - 2026", ws.Cell(2, 5).GetString());
        Assert.Equal(163.0, ws.Cell(2, 6).GetDouble(), 3);
        Assert.Equal("", ws.Cell(2, 7).GetString()); // null → blank
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch { }
    }
}
