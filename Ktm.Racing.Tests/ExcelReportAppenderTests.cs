using ClosedXML.Excel;
using Ktm.Racing.Core;
using Xunit;

namespace Ktm.Racing.Tests;

public class ExcelReportAppenderTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), $"rrappend_{Guid.NewGuid():N}");

    public ExcelReportAppenderTests() => Directory.CreateDirectory(_dir);

    private static readonly string DegC = "\u00b0C";

    // Result -> headers: Track, Session, Driver, Device, Ses.Date, "TOil_max [°C]", "nEngine_max [rpm]"
    private static RaceReportResult Result() => new()
    {
        Track = "T", Session = "R1", Driver = "D", Device = "AA 01",
        SessionDate = new DateTime(2026, 5, 31),
        Values = new[]
        {
            new RaceReportValue("TOil_max", DegC, 163.0),
            new RaceReportValue("nEngine_max", "rpm", 12000.0),
        },
    };

    private static string[] FullHeaders() =>
        new[] { "Track", "Session", "Driver", "Device", "Ses.Date", $"TOil_max [{DegC}]", "nEngine_max [rpm]" };

    private string CreateWorkbook(string[] headers, bool withDataRow = false)
    {
        var path = Path.Combine(_dir, $"target_{Guid.NewGuid():N}.xlsx");
        using var workbook = new XLWorkbook();
        var ws = workbook.AddWorksheet("Sheet1");
        for (int c = 0; c < headers.Length; c++) ws.Cell(1, c + 1).Value = headers[c];
        if (withDataRow)
            for (int c = 0; c < headers.Length; c++) ws.Cell(2, c + 1).Value = "existing";
        workbook.SaveAs(path);
        return path;
    }

    [Fact]
    public void Append_FullHeaderMatch_WritesRowAfterLastData()
    {
        var path = CreateWorkbook(FullHeaders(), withDataRow: true); // header row 1, data row 2

        var result = new ExcelReportAppender().Append(Result(), path);

        Assert.Equal(AppendStatus.Success, result.Status);
        Assert.Equal(3, result.AppendedRowIndex);
        Assert.NotNull(result.BackupPath);
        using var wb = new XLWorkbook(path);
        var ws = wb.Worksheet(1);
        Assert.Equal("T", ws.Cell(3, 1).GetString());
        Assert.Equal(163.0, ws.Cell(3, 6).GetDouble(), 3);
        Assert.Equal(12000.0, ws.Cell(3, 7).GetDouble(), 3);
    }

    [Fact]
    public void Append_ReorderedColumns_MatchesByHeaderName()
    {
        var headers = new[] { "nEngine_max [rpm]", "Track", $"TOil_max [{DegC}]", "Session", "Driver", "Device", "Ses.Date" };
        var path = CreateWorkbook(headers);

        var result = new ExcelReportAppender().Append(Result(), path);

        Assert.Equal(AppendStatus.Success, result.Status);
        using var wb = new XLWorkbook(path);
        var ws = wb.Worksheet(1);
        Assert.Equal(12000.0, ws.Cell(2, 1).GetDouble(), 3); // nEngine_max is column 1
        Assert.Equal(163.0, ws.Cell(2, 3).GetDouble(), 3);   // TOil_max is column 3
    }

    [Fact]
    public void Append_NoMatchingHeaders_ReturnsFailed()
    {
        var path = CreateWorkbook(new[] { "Alpha", "Beta", "Gamma" });

        var result = new ExcelReportAppender().Append(Result(), path);

        Assert.Equal(AppendStatus.Failed, result.Status);
        Assert.Contains("matching", result.ErrorMessage!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Append_UnknownExtraColumns_LeftUntouched()
    {
        var headers = FullHeaders().Append("Notes").ToArray(); // extra 8th column
        var path = CreateWorkbook(headers);

        var result = new ExcelReportAppender().Append(Result(), path);

        Assert.Equal(AppendStatus.Success, result.Status);
        using var wb = new XLWorkbook(path);
        var ws = wb.Worksheet(1);
        Assert.Equal("Notes", ws.Cell(1, 8).GetString());   // header intact
        Assert.Equal("", ws.Cell(2, 8).GetString());        // data cell untouched
    }

    [Fact]
    public void Append_NewCalculatedValueNotInFile_AddsColumn()
    {
        var headers = new[] { "Track", "Session", "Driver", "Device", "Ses.Date", $"TOil_max [{DegC}]" }; // nEngine_max missing
        var path = CreateWorkbook(headers);

        var result = new ExcelReportAppender().Append(Result(), path);

        Assert.Equal(AppendStatus.PartialSuccess, result.Status);
        Assert.Contains("nEngine_max [rpm]", result.AddedColumns);
        using var wb = new XLWorkbook(path);
        var ws = wb.Worksheet(1);
        Assert.Equal("nEngine_max [rpm]", ws.Cell(1, 7).GetString()); // added at end
        Assert.Equal(12000.0, ws.Cell(2, 7).GetDouble(), 3);
    }

    [Fact]
    public void Append_DuplicateHeaders_UsesFirst_AndWarns()
    {
        var headers = new[] { "Track", "Track", "Session", "Driver", "Device", "Ses.Date", $"TOil_max [{DegC}]", "nEngine_max [rpm]" };
        var path = CreateWorkbook(headers);

        var result = new ExcelReportAppender().Append(Result(), path);

        Assert.True(result.IsSuccess);
        Assert.Contains(result.Warnings, w => w.Contains("Duplicate", StringComparison.OrdinalIgnoreCase));
        using var wb = new XLWorkbook(path);
        var ws = wb.Worksheet(1);
        Assert.Equal("T", ws.Cell(2, 1).GetString());  // first Track written
        Assert.Equal("", ws.Cell(2, 2).GetString());   // second Track untouched
    }

    [Fact]
    public void Append_LockedFile_ReturnsFailed()
    {
        var path = CreateWorkbook(FullHeaders());
        using var _ = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

        var result = new ExcelReportAppender().Append(Result(), path);

        Assert.Equal(AppendStatus.Failed, result.Status);
        Assert.Contains("open", result.ErrorMessage!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Append_MissingFile_ReturnsFailed()
        => Assert.Equal(AppendStatus.Failed,
            new ExcelReportAppender().Append(Result(), Path.Combine(_dir, "nope.xlsx")).Status);

    [Fact]
    public void Append_WrongExtension_ReturnsFailed()
    {
        var txt = Path.Combine(_dir, "file.txt");
        File.WriteAllText(txt, "x");
        Assert.Equal(AppendStatus.Failed, new ExcelReportAppender().Append(Result(), txt).Status);
    }

    [Fact]
    public void Append_EmptySheet_CreatesHeaderRowAndData()
    {
        var path = Path.Combine(_dir, $"empty_{Guid.NewGuid():N}.xlsx");
        using (var wb = new XLWorkbook()) { wb.AddWorksheet("Sheet1"); wb.SaveAs(path); }

        var result = new ExcelReportAppender().Append(Result(), path);

        Assert.True(result.IsSuccess);
        using var wb2 = new XLWorkbook(path);
        var ws = wb2.Worksheet(1);
        Assert.Equal("Track", ws.Cell(1, 1).GetString());
        Assert.Equal(163.0, ws.Cell(2, 6).GetDouble(), 3);
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch { }
    }
}
