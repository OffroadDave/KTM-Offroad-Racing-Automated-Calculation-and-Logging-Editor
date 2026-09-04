using System.Diagnostics;
using System.Globalization;
using ClosedXML.Excel;

namespace Ktm.Racing.Core;

/// <summary>
/// Schreibt einen <see cref="RaceReportResult"/> nach Excel (ClosedXML):
/// entweder als neue Datei ({Track}_{Session}_{Device}.xlsx) oder als neue Zeile in einer bestehenden Liste.
/// Spalten: 5 Metadaten (Track|Session|Driver|Device|Ses.Date) gefolgt von den berechneten Kanälen.
/// </summary>
public sealed class RaceReportExcelWriter
{
    private static readonly string[] MetaHeaders = { "Track", "Session", "Driver", "Device", "Ses.Date" };

    /// <summary>Neue Datei erzeugen, optional öffnen; gibt den geschriebenen Pfad zurück.</summary>
    public string WriteNew(RaceReportResult result, string outputFolder, bool autoOpen = true)
    {
        Directory.CreateDirectory(outputFolder);
        var fileName = Sanitize($"{result.Track}_{result.Session}_{result.Device}") + ".xlsx";
        var path = UniquePath(Path.Combine(outputFolder, fileName));

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.AddWorksheet("RaceReport");
            var headers = BuildHeaders(result);
            for (int c = 0; c < headers.Count; c++)
                worksheet.Cell(1, c + 1).Value = headers[c];
            worksheet.Row(1).Style.Font.Bold = true;

            var values = BuildRow(result);
            for (int c = 0; c < values.Count; c++)
                SetCell(worksheet.Cell(2, c + 1), values[c]);

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(path);
        }

        if (autoOpen) TryOpen(path);
        return path;
    }

    /// <summary>Werte als neue Zeile an eine bestehende Excel anhängen (Zuordnung per Header-Name; Backup vorab).</summary>
    public string AppendRow(RaceReportResult result, string existingPath, bool autoOpen = true)
    {
        if (!File.Exists(existingPath))
            throw new FileNotFoundException($"Excel-Liste nicht gefunden: {existingPath}", existingPath);

        File.Copy(existingPath, $"{existingPath}.bak_{DateTime.Now:yyyyMMdd_HHmmss}", overwrite: true);

        using (var workbook = new XLWorkbook(existingPath))
        {
            var worksheet = workbook.Worksheets.First();
            var headerRow = worksheet.FirstRowUsed()
                ?? throw new InvalidDataException("Ziel-Excel hat keine Kopfzeile.");

            var columns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var cell in headerRow.CellsUsed())
                columns[cell.GetString().Trim()] = cell.Address.ColumnNumber;

            int targetRow = (worksheet.LastRowUsed()?.RowNumber() ?? headerRow.RowNumber()) + 1;

            var headers = BuildHeaders(result);
            var values = BuildRow(result);
            var missing = new List<string>();
            for (int i = 0; i < headers.Count; i++)
            {
                if (columns.TryGetValue(headers[i], out var col))
                    SetCell(worksheet.Cell(targetRow, col), values[i]);
                else
                    missing.Add(headers[i]);
            }
            if (missing.Count > 0)
                Console.WriteLine($"[excel] Warnung: Spalten nicht in Zielliste vorhanden: {string.Join(", ", missing)}");

            workbook.Save();
        }

        if (autoOpen) TryOpen(existingPath);
        return existingPath;
    }

    private static List<string> BuildHeaders(RaceReportResult result)
    {
        var list = new List<string>(MetaHeaders);
        foreach (var value in result.Values)
            list.Add(string.IsNullOrEmpty(value.Unit) ? value.Alias : $"{value.Alias} [{value.Unit}]");
        return list;
    }

    private static List<object?> BuildRow(RaceReportResult result)
    {
        var list = new List<object?>
        {
            result.Track, result.Session, result.Driver, result.Device,
            result.SessionDate?.ToString("dd - MMM - yyyy", CultureInfo.InvariantCulture) ?? "",
        };
        foreach (var value in result.Values) list.Add(value.Value);
        return list;
    }

    private static void SetCell(IXLCell cell, object? value)
    {
        switch (value)
        {
            case null: cell.Value = ""; break;
            case double d: cell.Value = d; break;
            default: cell.Value = value.ToString(); break;
        }
    }

    private static string Sanitize(string name)
    {
        foreach (var invalid in Path.GetInvalidFileNameChars()) name = name.Replace(invalid, '_');
        name = name.Trim();
        return string.IsNullOrEmpty(name) ? "RaceReport" : name;
    }

    private static string UniquePath(string path)
    {
        if (!File.Exists(path)) return path;
        var dir = Path.GetDirectoryName(path)!;
        var name = Path.GetFileNameWithoutExtension(path);
        var ext = Path.GetExtension(path);
        for (int i = 2; ; i++)
        {
            var candidate = Path.Combine(dir, $"{name}_{i}{ext}");
            if (!File.Exists(candidate)) return candidate;
        }
    }

    private static void TryOpen(string path)
    {
        try { Process.Start(new ProcessStartInfo(path) { UseShellExecute = true }); }
        catch (Exception ex) { Console.WriteLine($"[excel] Konnte Datei nicht öffnen: {ex.Message}"); }
    }
}
