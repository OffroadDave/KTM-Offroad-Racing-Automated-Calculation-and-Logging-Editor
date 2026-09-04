using Ktm.Racing.Core;
using Ktm.Racing.RaceReport.Mvvm;

namespace Ktm.Racing.RaceReport.ViewModels;

/// <summary>Zeilen-ViewModel für einen entdeckten Run inklusive Auswahl-Status.</summary>
public sealed class RunRowViewModel : ObservableObject
{
    private bool _isSelected = true;

    public RunRowViewModel(DiscoveredRun run) => Run = run;

    public DiscoveredRun Run { get; }

    public string RunName => Run.RunName;
    public string Device => Run.Device;
    public string Session => Run.Session;
    public int LapCount => Run.LapCount;
    public string StartTime => Run.StartTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";

    /// <summary>Ausgewählt für Race Report. Standard true (ganze Session verarbeiten).</summary>
    public bool IsSelected
    {
        get => _isSelected;
        set { if (SetProperty(ref _isSelected, value)) SelectionChanged?.Invoke(); }
    }

    /// <summary>Wird ausgelöst, wenn sich die Auswahl ändert (damit das MainViewModel HasSelection neu bewertet).</summary>
    public event Action? SelectionChanged;
}
