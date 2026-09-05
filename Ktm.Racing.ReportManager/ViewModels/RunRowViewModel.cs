using Ktm.Racing.Core;
using Ktm.Racing.RaceReport.Mvvm;

namespace Ktm.Racing.RaceReport.ViewModels;

/// <summary>Row ViewModel for a discovered run, including its selection state.</summary>
public sealed class RunRowViewModel : ObservableObject
{
    private bool _isSelected = false;

    public RunRowViewModel(DiscoveredRun run) => Run = run;

    public DiscoveredRun Run { get; }

    public string RunName => Run.RunName;
    public string Device => Run.Device;
    public string Session => Run.Session;
    public int LapCount => Run.LapCount;

    /// <summary>Total run duration = sum of all lap times, formatted as [h:]mm:ss.</summary>
    public string Duration => Run.Duration is { } d ? FormatDuration(d) : "";

    /// <summary>Real acquisition timestamp (from RunLapHeader AcquireTime); copy-safe, unlike the old ModT.</summary>
    public string AcquisitionTime => Run.AcquisitionTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";

    /// <summary>Stable, copy-safe acquisition order (smallest lap-folder counter); replaces the unreliable start time.</summary>
    public long? AcquisitionOrder => Run.AcquisitionOrder;

    /// <summary>Selected for the Race Report. Defaults to false; use "Select all" or click the checkbox.</summary>
    public bool IsSelected
    {
        get => _isSelected;
        set { if (SetProperty(ref _isSelected, value)) SelectionChanged?.Invoke(); }
    }

    /// <summary>Raised when the selection changes (so the MainViewModel can re-evaluate HasSelection).</summary>
    public event Action? SelectionChanged;

    private static string FormatDuration(TimeSpan d) =>
        d.TotalHours >= 1
            ? $"{(int)d.TotalHours}:{d.Minutes:00}:{d.Seconds:00}"
            : $"{d.Minutes:00}:{d.Seconds:00}";
}
