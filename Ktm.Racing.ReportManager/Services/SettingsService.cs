using System.IO;
using System.Text.Json;

namespace Ktm.Racing.RaceReport.Services;

/// <summary>
/// Loads and saves <see cref="ReportManagerSettings"/> as JSON under
/// %LOCALAPPDATA%\KtmRacingReportManager\settings.json. All I/O is best-effort:
/// failures never crash the UI, they simply fall back to defaults.
/// </summary>
public sealed class SettingsService
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    private readonly string _filePath;

    public SettingsService()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "KtmRacingReportManager");
        _filePath = Path.Combine(dir, "settings.json");
    }

    /// <summary>Reads the settings file; returns defaults if it is missing or unreadable.</summary>
    public ReportManagerSettings Load()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var settings = JsonSerializer.Deserialize<ReportManagerSettings>(json);
                if (settings is not null) return settings;
            }
        }
        catch
        {
            // Ignore and fall back to defaults - persisted preferences are non-critical.
        }
        return new ReportManagerSettings();
    }

    /// <summary>Writes the settings file (best-effort; never throws).</summary>
    public void Save(ReportManagerSettings settings)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            File.WriteAllText(_filePath, JsonSerializer.Serialize(settings, Options));
        }
        catch
        {
            // Ignore - persisting UI preferences must not disrupt the workflow.
        }
    }
}
