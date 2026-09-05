using ClosedXML.Excel;

namespace Ktm.Racing.Core;

/// <summary>
/// Appends a <see cref="RaceReportResult"/> as a new row to an existing .xlsx file.
///
/// Behavior:
///  - Columns are matched by exact header name (never by hard-coded index).
///  - Calculated fields whose header is missing are added as new columns at the end.
///  - Unknown/extra columns in the template are left untouched.
///  - A non-empty header row with zero matches is treated as the wrong file (fails).
///  - An empty worksheet gets a fresh header row (the user explicitly chose this file).
///  - Duplicate headers use the first occurrence and produce a warning.
///  - A timestamped backup is created before writing.
/// The header map is built once per call (O(number of headers)).
/// </summary>
public sealed class ExcelReportAppender : IReportAppender
{
    public AppendResult Append(RaceReportResult result, string targetPath, string? worksheetName = null)
    {
        if (string.IsNullOrWhiteSpace(targetPath))
            return Failed("No target file was provided.");
        if (!string.Equals(Path.GetExtension(targetPath), ".xlsx", StringComparison.OrdinalIgnoreCase))
            return Failed("The target file must be an .xlsx workbook.");
        if (!File.Exists(targetPath))
            return Failed($"The target file does not exist: {targetPath}");
        if (IsFileLocked(targetPath, out var lockError))
            return Failed(lockError!);

        var warnings = new List<string>();
        string? backupPath = null;
        try
        {
            backupPath = CreateBackup(targetPath);

            using var workbook = new XLWorkbook(targetPath);
            var worksheet = SelectWorksheet(workbook, worksheetName, warnings);
            if (worksheet is null)
                return Failed("The target workbook has no worksheet.", backupPath, warnings);

            var headers = ReportColumns.BuildHeaders(result);
            var values = ReportColumns.BuildRow(result);

            var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var headerRow = worksheet.FirstRowUsed();
            bool sheetWasEmpty = headerRow is null;
            int headerRowNumber = headerRow?.RowNumber() ?? 1;
            int nextFreeColumn = 1;

            if (!sheetWasEmpty)
            {
                foreach (var cell in headerRow!.CellsUsed())
                {
                    var name = cell.GetString().Trim();
                    if (name.Length == 0) continue;
                    if (headerMap.ContainsKey(name))
                        warnings.Add($"Duplicate header '{name}' found; the first occurrence is used.");
                    else
                        headerMap[name] = cell.Address.ColumnNumber;
                }
                nextFreeColumn = (headerRow.LastCellUsed()?.Address.ColumnNumber ?? 0) + 1;

                // A populated header row with no matching column is almost certainly the wrong file.
                if (!headers.Any(headerMap.ContainsKey))
                    return Failed("No matching columns were found in the target file.", backupPath, warnings);
            }
            else
            {
                warnings.Add("The target worksheet was empty; a header row was created.");
            }

            int targetRow = (worksheet.LastRowUsed()?.RowNumber() ?? headerRowNumber) + 1;

            var matched = new List<string>();
            var added = new List<string>();
            for (int i = 0; i < headers.Count; i++)
            {
                var header = headers[i];
                if (headerMap.TryGetValue(header, out var column))
                {
                    ReportColumns.SetCell(worksheet.Cell(targetRow, column), values[i]);
                    matched.Add(header);
                }
                else
                {
                    worksheet.Cell(headerRowNumber, nextFreeColumn).Value = header;
                    ReportColumns.SetCell(worksheet.Cell(targetRow, nextFreeColumn), values[i]);
                    headerMap[header] = nextFreeColumn;
                    added.Add(header);
                    if (!sheetWasEmpty)
                        warnings.Add($"Column '{header}' was not present and has been added.");
                    nextFreeColumn++;
                }
            }

            workbook.Save();

            return new AppendResult
            {
                Status = warnings.Count == 0 ? AppendStatus.Success : AppendStatus.PartialSuccess,
                AppendedRowIndex = targetRow,
                MatchedColumns = matched,
                AddedColumns = added,
                Warnings = warnings,
                BackupPath = backupPath,
            };
        }
        catch (IOException ex)
        {
            return Failed($"The file is in use or an I/O error occurred: {ex.Message}", backupPath, warnings);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Failed($"Access to the file was denied: {ex.Message}", backupPath, warnings);
        }
        catch (Exception ex)
        {
            return Failed($"Appending to the workbook failed: {ex.Message}", backupPath, warnings);
        }
    }

    private static bool IsFileLocked(string path, out string? error)
    {
        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            error = null;
            return false;
        }
        catch (IOException)
        {
            error = "The target file is currently open in another program (e.g. Excel). Please close it and try again.";
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            error = "Access to the target file was denied. Please check the file permissions.";
            return true;
        }
    }

    private static string CreateBackup(string path)
    {
        var backup = $"{path}.bak_{DateTime.Now:yyyyMMdd_HHmmss}";
        File.Copy(path, backup, overwrite: true);
        return backup;
    }

    private static IXLWorksheet? SelectWorksheet(XLWorkbook workbook, string? name, List<string> warnings)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            if (workbook.Worksheets.TryGetWorksheet(name, out var named)) return named;
            warnings.Add($"Worksheet '{name}' was not found; the first worksheet is used.");
        }
        return workbook.Worksheets.FirstOrDefault();
    }

    private static AppendResult Failed(string message, string? backupPath = null, List<string>? warnings = null)
        => new()
        {
            Status = AppendStatus.Failed,
            ErrorMessage = message,
            BackupPath = backupPath,
            Warnings = warnings ?? new List<string>(),
        };
}
