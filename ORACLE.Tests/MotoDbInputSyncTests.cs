using ClosedXML.Excel;

namespace Ktm.Racing.Oracle.Tests;

public sealed class MotoDbInputSyncTests
{
    [Fact]
    public void Run_returns_DataNotFound_when_kompass_missing()
    {
        using var tmp = new TempDir();
        var workbook = CreateWorkbook(tmp);
        var code = MotoDbInputSync.Run(new OracleConfig(), Path.Combine(tmp.Path, "nope.json"), workbook);
        Assert.Equal(OracleExitCode.DataNotFound, code);
    }

    [Fact]
    public void Run_returns_DataNotFound_when_workbook_missing()
    {
        using var tmp = new TempDir();
        var kompass = CreateKompass(tmp);
        var code = MotoDbInputSync.Run(new OracleConfig(), kompass, Path.Combine(tmp.Path, "nope.xlsx"));
        Assert.Equal(OracleExitCode.DataNotFound, code);
    }

    [Fact]
    public void Run_returns_ResourceLocked_when_workbook_is_locked()
    {
        using var tmp = new TempDir();
        var kompass = CreateKompass(tmp);
        var workbook = CreateWorkbook(tmp);

        // Workbook exists but is held open (as if open in Excel) -> transient lock, not a clean skip.
        using (new FileStream(workbook, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            var code = MotoDbInputSync.Run(new OracleConfig { TrackLength = new TrackLengthSettings { LockWaitSeconds = 0 } }, kompass, workbook);
            Assert.Equal(OracleExitCode.ResourceLocked, code);
        }
    }

    [Fact]
    public void Run_syncs_circuit_from_kompass_and_returns_Success()
    {
        using var tmp = new TempDir();
        var kompass = CreateKompass(tmp);
        var workbook = CreateWorkbook(tmp);

        var code = MotoDbInputSync.Run(new OracleConfig(), kompass, workbook);

        Assert.Equal(OracleExitCode.Success, code);

        using var wb = new XLWorkbook(workbook);
        var circuits = wb.Worksheet("CIRCUITS");
        var last = circuits.LastRowUsed()!.RowNumber();
        var found = false;
        for (var r = 2; r <= last; r++)
        {
            if (string.Equals(circuits.Cell(r, 1).GetString(), "Lommel", StringComparison.OrdinalIgnoreCase))
            {
                found = true;
                Assert.Equal(1850, (int)circuits.Cell(r, 9).GetDouble());
            }
        }
        Assert.True(found);
    }

    private static string CreateKompass(TempDir tmp) =>
        tmp.Write("KOMPASS.json",
            """{ "MX": { "Tracks": [ { "Track": "Lommel", "Country": "Belgium", "Countrycode": "BEL", "Tracklength": "1850", "Surface": "Sand" } ] } }""");

    private static string CreateWorkbook(TempDir tmp)
    {
        var path = Path.Combine(tmp.Path, "loadChannels.xlsx");
        using var wb = new XLWorkbook();
        wb.Worksheets.Add("MX-Factory-450").Cell(1, 1).Value = "channels"; // MotoDB-owned sheet must survive
        var circuits = wb.Worksheets.Add("CIRCUITS");
        circuits.Cell(1, 1).Value = "Name";
        circuits.Cell(1, 9).Value = "Length";
        var hed = wb.Worksheets.Add("HED");
        hed.Cell(1, 1).Value = "Group";
        hed.Cell(1, 4).Value = "NAME";
        wb.SaveAs(path);
        return path;
    }
}
