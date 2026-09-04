using ClosedXML.Excel;

namespace Ktm.Racing.Core;

/// <summary>
/// Liest die Input-Excel (Racereport_Inputs_MX.xlsx): jede Zeile mit LOAD=1 und CALC=1 ergibt einen
/// zu berechnenden Kanal. Spalten werden per Header-Name adressiert (LOAD, Channel, Alias, CALC, Unit).
/// </summary>
public sealed class RaceReportInputReader
{
    public IReadOnlyList<ChannelInput> Read(string xlsxPath, string? sheetName = null)
    {
        if (!File.Exists(xlsxPath))
            throw new FileNotFoundException($"Input-Excel nicht gefunden: {xlsxPath}", xlsxPath);

        // FileShare.ReadWrite: Datei auch dann lesen, wenn sie parallel in Excel geöffnet ist.
        using var stream = new FileStream(xlsxPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var workbook = new XLWorkbook(stream);
        var worksheet = (!string.IsNullOrWhiteSpace(sheetName) && workbook.Worksheets.TryGetWorksheet(sheetName, out var named))
            ? named
            : workbook.Worksheets.First();

        var headerRow = worksheet.FirstRowUsed()
            ?? throw new InvalidDataException("Input-Excel: keine Kopfzeile gefunden.");

        var columns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var cell in headerRow.CellsUsed())
            columns[cell.GetString().Trim()] = cell.Address.ColumnNumber;

        int Col(string name, int fallback) => columns.TryGetValue(name, out var c) ? c : fallback;
        int loadCol = Col("LOAD", 1), channelCol = Col("Channel", 3), aliasCol = Col("Alias", 5),
            calcCol = Col("CALC", 10), unitCol = Col("Unit", 11);

        var result = new List<ChannelInput>();
        foreach (var row in worksheet.RowsUsed().Skip(1))
        {
            if (!IsActive(row.Cell(loadCol)) || !IsActive(row.Cell(calcCol))) continue;

            var channel = row.Cell(channelCol).GetString().Trim();
            if (string.IsNullOrEmpty(channel)) continue;

            var alias = row.Cell(aliasCol).GetString().Trim();
            if (string.IsNullOrEmpty(alias)) alias = channel;

            var unit = unitCol > 0 ? row.Cell(unitCol).GetString().Trim() : "";
            result.Add(new ChannelInput(channel, alias, unit));
        }
        return result;
    }

    private static bool IsActive(IXLCell cell)
    {
        if (cell.TryGetValue<double>(out var d)) return Math.Abs(d - 1.0) < 0.5;
        return cell.GetString().Trim() == "1";
    }
}
