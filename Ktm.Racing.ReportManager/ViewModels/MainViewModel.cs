using System.Collections.ObjectModel;
using System.IO;
using Ktm.Racing.Core;
using Ktm.Racing.RaceReport.Mvvm;
using Microsoft.Win32;

namespace Ktm.Racing.RaceReport.ViewModels;

/// <summary>
/// Hauptfenster-ViewModel: Ordnerauswahl, Run-Discovery (Hintergrund-Thread), Mehrfachauswahl
/// und die Aktionen "Race Report" (Pipeline folgt in Phase 3) und "Run ORACLE".
/// </summary>
public sealed class MainViewModel : ObservableObject
{
    private readonly RunDiscoveryService _discovery = new();
    private readonly RaceReportService _raceReport = new();
    private readonly RaceReportExcelWriter _excelWriter = new();

    private string _selectedFolder = "";
    private bool _isBusy;
    private string _statusText = "Bitte einen Session- oder Track-Ordner wählen.";

    public MainViewModel()
    {
        BrowseCommand = new RelayCommand(Browse, () => !IsBusy);
        RefreshCommand = new AsyncRelayCommand(DiscoverAsync, () => !IsBusy && Directory.Exists(SelectedFolder));
        RaceReportCommand = new AsyncRelayCommand(RaceReportAsync, () => !IsBusy && HasSelection);
        RunOracleCommand = new AsyncRelayCommand(RunOracleAsync, () => !IsBusy && Directory.Exists(SelectedFolder));
    }

    public ObservableCollection<RunRowViewModel> Runs { get; } = new();

    public RelayCommand BrowseCommand { get; }
    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand RaceReportCommand { get; }
    public AsyncRelayCommand RunOracleCommand { get; }

    public string SelectedFolder
    {
        get => _selectedFolder;
        set
        {
            if (SetProperty(ref _selectedFolder, value))
            {
                RefreshCommand.RaiseCanExecuteChanged();
                RunOracleCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set { if (SetProperty(ref _isBusy, value)) RaiseAllCanExecute(); }
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    /// <summary>Mindestens ein Run ausgewählt → "Race Report" ist zulässig.</summary>
    public bool HasSelection => Runs.Any(r => r.IsSelected);

    private void RaiseAllCanExecute()
    {
        BrowseCommand.RaiseCanExecuteChanged();
        RefreshCommand.RaiseCanExecuteChanged();
        RaceReportCommand.RaiseCanExecuteChanged();
        RunOracleCommand.RaiseCanExecuteChanged();
    }

    private void Browse()
    {
        var dialog = new OpenFolderDialog { Title = "Session- oder Track-Ordner wählen" };
        if (Directory.Exists(SelectedFolder)) dialog.InitialDirectory = SelectedFolder;
        if (dialog.ShowDialog() == true)
        {
            SelectedFolder = dialog.FolderName;
            _ = DiscoverAsync();
        }
    }

    private async Task DiscoverAsync()
    {
        if (!Directory.Exists(SelectedFolder)) { StatusText = "Ordner existiert nicht."; return; }

        IsBusy = true;
        try
        {
            ClearRuns();
            var folder = SelectedFolder;
            var progress = new Progress<string>(message => StatusText = message);

            var found = await Task.Run(() => _discovery.Discover(folder, progress));
            foreach (var run in found)
            {
                var row = new RunRowViewModel(run);
                row.SelectionChanged += OnRunSelectionChanged;
                Runs.Add(row);
            }

            OnRunSelectionChanged();
            RunOracleCommand.RaiseCanExecuteChanged();
            StatusText = found.Count == 0
                ? "Keine Runs im gewählten Ordner gefunden."
                : $"{found.Count} Run(s) gefunden – Auswahl treffen und 'Race Report' drücken.";
        }
        catch (Exception ex)
        {
            StatusText = $"Fehler bei der Run-Suche: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ClearRuns()
    {
        foreach (var row in Runs) row.SelectionChanged -= OnRunSelectionChanged;
        Runs.Clear();
        OnRunSelectionChanged();
    }

    private void OnRunSelectionChanged()
    {
        OnPropertyChanged(nameof(HasSelection));
        RaceReportCommand.RaiseCanExecuteChanged();
    }

    private async Task RaceReportAsync()
    {
        var selected = Runs.Where(r => r.IsSelected).Select(r => r.Run).ToList();
        if (selected.Count == 0) return;

        IsBusy = true;
        try
        {
            var progress = new Progress<string>(message => StatusText = message);
            var (fileName, channelCount, warnings) = await Task.Run(() =>
            {
                var result = _raceReport.Generate(selected, progress);
                var outputFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                var path = _excelWriter.WriteNew(result, outputFolder, autoOpen: true);
                return (Path.GetFileName(path), result.Values.Count, result.Warnings);
            });
            var warn = warnings.Count > 0 ? $" | Hinweise: {string.Join("; ", warnings)}" : "";
            StatusText = $"Race Report erstellt: {fileName} ({channelCount} Kanäle){warn}";
        }
        catch (Exception ex)
        {
            StatusText = $"Fehler beim Race Report: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RunOracleAsync()
    {
        var folder = SelectedFolder;
        if (!Directory.Exists(folder)) { StatusText = "Bitte zuerst einen gültigen Ordner wählen."; return; }

        IsBusy = true;
        try
        {
            var progress = new Progress<string>(message => StatusText = message);
            // Verarbeitet ALLE Runs im gewählten Track-/Session-Ordner: process je Run + merge je Rider.
            var code = await Task.Run(() => OraclePipeline.Create().ProcessFolderAsync(folder, force: false, progress));
            StatusText = $"ORACLE abgeschlossen: {code}. 'Aktualisieren' drücken, um die aktualisierten Runs zu laden.";
        }
        catch (Exception ex)
        {
            StatusText = $"ORACLE-Fehler: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
