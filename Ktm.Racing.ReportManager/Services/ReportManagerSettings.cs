namespace Ktm.Racing.RaceReport.Services;

/// <summary>Persisted user choices for the Report Manager UI.</summary>
public sealed class ReportManagerSettings
{
    /// <summary>When true, the Race Report is appended to an existing workbook instead of creating a new file.</summary>
    public bool AppendMode { get; set; }

    /// <summary>Absolute path to the target .xlsx workbook used when <see cref="AppendMode"/> is enabled.</summary>
    public string AppendTargetPath { get; set; } = "";
}
