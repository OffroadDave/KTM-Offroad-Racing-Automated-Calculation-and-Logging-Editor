using System.Diagnostics;
using ClosedXML.Excel;

namespace Ktm.Racing.Core;

/// <summary>
/// Writes a <see cref="RaceReportResult"/> to a new Excel file ({Track}_{Session}_{Device}.xlsx).
/// Columns: five metadata columns (Track | Session | Driver | Device | Ses.Date) followed by the calculated
/// channels. Appending to an existing file is handled by <see cref="ExcelReportAppender"/>.
/// </summary>
public sealed class RaceReportExcelWriter
{
    /// <summary>Creates a new file, optionally opens it, and returns the written path.</summary>
    public string WriteNew(RaceReportResult result, string outputFolder, bool autoOpen = true)
    {
        Directory.CreateDirectory(outputFolder);
        var fileName = Sanitize($"{result.Track}_{result.Session}_{result.Device}") + ".xlsx";
        var path = UniquePath(Path.Combine(outputFolder, fileName));

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.AddWorksheet("RaceReport");
            var headers = ReportColumns.BuildHeaders(result);
            for (int c = 0; c < headers.Count; c++)
                worksheet.Cell(1, c + 1).Value = headers[c];
            worksheet.Row(1).Style.Font.Bold = true;

            var values = ReportColumns.BuildRow(result);
            for (int c = 0; c < values.Count; c++)
                ReportColumns.SetCell(worksheet.Cell(2, c + 1), values[c]);

            // Number format per channel from the VCH decimals (new file only; append keeps the target's formatting).
            for (int i = 0; i < result.Values.Count; i++)
                if (result.Values[i] is { Value: not null, Decimals: int decimals })
                    worksheet.Cell(2, ReportColumns.MetaHeaders.Length + i + 1).Style.NumberFormat.Format =
                        ReportColumns.NumberFormat(decimals);

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(path);
        }

        if (autoOpen) TryOpen(path);
        return path;
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
        catch (Exception ex) { Console.WriteLine($"[excel] Could not open the file: {ex.Message}"); }
    }
}
