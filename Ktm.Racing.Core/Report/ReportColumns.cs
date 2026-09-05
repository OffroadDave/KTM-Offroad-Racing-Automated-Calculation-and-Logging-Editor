using System.Globalization;
using ClosedXML.Excel;

namespace Ktm.Racing.Core;

/// <summary>
/// Canonical mapping between a <see cref="RaceReportResult"/> and its Excel columns:
/// five metadata columns (Track | Session | Driver | Device | Ses.Date) followed by one column per
/// calculated channel ("{Alias} [{Unit}]"). Shared by the "create new" writer and the "append" service
/// so both produce identical headers.
/// </summary>
internal static class ReportColumns
{
    public static readonly string[] MetaHeaders = { "Track", "Session", "Driver", "Device", "Ses.Date" };

    /// <summary>Ordered header names for the result (metadata + one per channel).</summary>
    public static List<string> BuildHeaders(RaceReportResult result)
    {
        var headers = new List<string>(MetaHeaders);
        foreach (var value in result.Values)
            headers.Add(string.IsNullOrEmpty(value.Unit) ? value.Alias : $"{value.Alias} [{value.Unit}]");
        return headers;
    }

    /// <summary>Ordered row values aligned with <see cref="BuildHeaders"/>.</summary>
    public static List<object?> BuildRow(RaceReportResult result)
    {
        var values = new List<object?>
        {
            result.Track, result.Session, result.Driver, result.Device,
            result.SessionDate?.ToString("dd - MMM - yyyy", CultureInfo.InvariantCulture) ?? "",
        };
        foreach (var value in result.Values) values.Add(value.Value);
        return values;
    }

    /// <summary>Writes a value into a cell as number, text, or blank (null).</summary>
    public static void SetCell(IXLCell cell, object? value)
    {
        switch (value)
        {
            case null: cell.Value = ""; break;
            case double d: cell.Value = d; break;
            default: cell.Value = value.ToString(); break;
        }
    }

    /// <summary>Excel number format for n decimals ("0", "0.0", "0.00", …), per the VCH Decimals setting.</summary>
    public static string NumberFormat(int decimals) =>
        decimals <= 0 ? "0" : "0." + new string('0', decimals);
}
