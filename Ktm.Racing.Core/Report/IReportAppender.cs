namespace Ktm.Racing.Core;

/// <summary>Outcome of an append operation.</summary>
public enum AppendStatus
{
    Success,
    PartialSuccess,
    Failed,
}

/// <summary>Structured result of appending a report row to an existing workbook.</summary>
public sealed class AppendResult
{
    public AppendStatus Status { get; init; }

    /// <summary>1-based row index that received the appended data (0 when the append failed).</summary>
    public int AppendedRowIndex { get; init; }

    /// <summary>Headers that matched an existing column and were written.</summary>
    public IReadOnlyList<string> MatchedColumns { get; init; } = Array.Empty<string>();

    /// <summary>Headers that were not present and were added as new columns.</summary>
    public IReadOnlyList<string> AddedColumns { get; init; } = Array.Empty<string>();

    /// <summary>Non-fatal warnings (duplicate headers, added columns, worksheet fallback, …).</summary>
    public IReadOnlyList<string> Warnings { get; init; } = Array.Empty<string>();

    /// <summary>Path of the safety backup created before writing (null if none).</summary>
    public string? BackupPath { get; init; }

    /// <summary>User-facing error message when <see cref="Status"/> is <see cref="AppendStatus.Failed"/>.</summary>
    public string? ErrorMessage { get; init; }

    public bool IsSuccess => Status != AppendStatus.Failed;
}

/// <summary>Appends a calculated report as a new row into an existing Excel file, matching columns by header name.</summary>
public interface IReportAppender
{
    AppendResult Append(RaceReportResult result, string targetPath, string? worksheetName = null);
}
