using ClosedXML.Excel;
using Ktm.Racing.Core;
using Xunit;

namespace Ktm.Racing.Tests;

public class RaceReportInputReaderTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), $"rrinput_{Guid.NewGuid():N}.xlsx");

    private void CreateWorkbook()
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.AddWorksheet("MX-Factory-450");
        ws.Cell(1, 1).Value = "LOAD"; ws.Cell(1, 3).Value = "Channel"; ws.Cell(1, 5).Value = "Alias";
        ws.Cell(1, 10).Value = "CALC"; ws.Cell(1, 11).Value = "Unit";

        // Active channel.
        ws.Cell(2, 1).Value = 1; ws.Cell(2, 3).Value = "TOil_max"; ws.Cell(2, 5).Value = "TOil_max";
        ws.Cell(2, 10).Value = 1; ws.Cell(2, 11).Value = "\u00b0C";
        // LOAD = 0 → excluded.
        ws.Cell(3, 1).Value = 0; ws.Cell(3, 3).Value = "TEng_max"; ws.Cell(3, 5).Value = "TEng_max";
        ws.Cell(3, 10).Value = 1; ws.Cell(3, 11).Value = "\u00b0C";
        // CALC = 0 → excluded.
        ws.Cell(4, 1).Value = 1; ws.Cell(4, 3).Value = "TAir_max"; ws.Cell(4, 5).Value = "TAir_max";
        ws.Cell(4, 10).Value = 0; ws.Cell(4, 11).Value = "\u00b0C";
        // Active, no alias → falls back to channel name.
        ws.Cell(5, 1).Value = 1; ws.Cell(5, 3).Value = "nEngine_max"; ws.Cell(5, 10).Value = 1; ws.Cell(5, 11).Value = "rpm";

        workbook.SaveAs(_path);
    }

    [Fact]
    public void Read_ReturnsOnlyActiveChannels_WithAliasFallback()
    {
        CreateWorkbook();
        var inputs = new RaceReportInputReader().Read(_path);

        Assert.Equal(2, inputs.Count);
        Assert.Equal("TOil_max", inputs[0].Channel);
        Assert.Equal("\u00b0C", inputs[0].Unit);
        Assert.Equal("nEngine_max", inputs[1].Channel);
        Assert.Equal("nEngine_max", inputs[1].Alias); // fallback = channel name
        Assert.Equal("rpm", inputs[1].Unit);
    }

    public void Dispose()
    {
        try { File.Delete(_path); } catch { }
    }
}
