using ClosedXML.Excel;
using Ktm.Racing.Core;
using Xunit;

namespace Ktm.Racing.Tests;

/// <summary>
/// Integrationstests gegen echte Testdaten + KtmDataHandler-DLLs. Werden übersprungen (nicht als Fehler
/// gewertet), wenn die Ressourcen (Testdaten, DLL-Suite, Input-Excel) nicht vorhanden sind.
/// </summary>
public class IntegrationTests
{
    private const string TestData =
        @"C:\MXTrackData\Testdata\01_KTM-FR\Wintax\2026\450\05_RT\RT22_GER-Teutschenthal\R1\AA 01";
    private const string Suite = @"C:\KTM Tools\KTM Utils Software Suite";

    private static bool DataAndSuite => Directory.Exists(TestData) && Directory.Exists(Suite);

    [SkippableFact]
    public void Discover_FindsAllRuns_WithMetadata_ChronologicallyOrdered()
    {
        Skip.IfNot(DataAndSuite, "Testdaten oder KtmDataHandler-Suite nicht verfügbar.");

        var runs = new RunDiscoveryService().Discover(TestData);

        Assert.Equal(10, runs.Count);
        Assert.All(runs, r => Assert.Equal("AA 01", r.Device));
        Assert.All(runs, r => Assert.Equal("R1", r.Session));
        Assert.All(runs, r => Assert.Equal("RT22_GER-Teutschenthal", r.Track));

        Assert.Equal("Run_1078", runs[0].RunName); // smallest lap counter first
        var orders = runs.Where(r => r.AcquisitionOrder.HasValue).Select(r => r.AcquisitionOrder!.Value).ToList();
        Assert.Equal(orders.OrderBy(x => x), orders); // ascending acquisition order
    }

    [SkippableFact]
    public void Generate_SingleRaceRun_ProducesPlausibleAggregates()
    {
        Skip.IfNot(DataAndSuite, "Testdaten oder KtmDataHandler-Suite nicht verfügbar.");

        // Eigene Input-Excel (unabhängig von der geteilten, ggf. angepassten Datei).
        var inputPath = WriteTestInput("TOil_max", "nEngine_max", "TEng_ActiveLC_avg");
        try
        {
            var options = new RaceReportOptions { InputExcelPath = inputPath };
            var runs = new RunDiscoveryService().Discover(TestData);
            var race = runs.OrderByDescending(r => r.LapCount).First(); // Run_1084 (16 laps)

            var result = new RaceReportService(options).Generate(new[] { race });

            Assert.Equal("RT22_GER-Teutschenthal", result.Track);
            Assert.Equal("R1", result.Session);

            var oilMax = result.Values.First(v => v.Alias == "TOil_max").Value;
            Assert.NotNull(oilMax);
            Assert.InRange(oilMax!.Value, 100, 200); // plausible oil temperature

            var rpmMax = result.Values.First(v => v.Alias == "nEngine_max").Value;
            Assert.NotNull(rpmMax);
            Assert.InRange(rpmMax!.Value, 8000, 15000); // plausible max engine rpm

            // Launch control never active in this data → blank.
            Assert.Null(result.Values.First(v => v.Alias == "TEng_ActiveLC_avg").Value);
        }
        finally
        {
            File.Delete(inputPath);
        }
    }

    // Erzeugt eine minimale Input-Excel im neuen Spaltenlayout (CALC=G, Unit=I) mit Alias = Channel.
    private static string WriteTestInput(params string[] channels)
    {
        var path = Path.Combine(Path.GetTempPath(), $"rrin_{Guid.NewGuid():N}.xlsx");
        using var workbook = new XLWorkbook();
        var ws = workbook.AddWorksheet("MX-Factory-450");
        ws.Cell(1, 1).Value = "LOAD"; ws.Cell(1, 3).Value = "Channel"; ws.Cell(1, 5).Value = "Alias";
        ws.Cell(1, 7).Value = "CALC"; ws.Cell(1, 9).Value = "Unit";
        int row = 2;
        foreach (var channel in channels)
        {
            ws.Cell(row, 1).Value = 1; ws.Cell(row, 3).Value = channel; ws.Cell(row, 5).Value = channel;
            ws.Cell(row, 7).Value = 1; row++;
        }
        workbook.SaveAs(path);
        return path;
    }
}
