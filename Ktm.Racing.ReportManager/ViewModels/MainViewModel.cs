using System.Collections.ObjectModel;
using System.IO;
using Ktm.Racing.Core;
using Ktm.Racing.RaceReport.Mvvm;
using Ktm.Racing.RaceReport.Services;
using Microsoft.Win32;

namespace Ktm.Racing.RaceReport.ViewModels;

/// <summary>
/// Main-window ViewModel: folder selection, run discovery (background thread), multi-selection,
/// and the actions "Race Report" (create new file or append to an existing workbook) and "Run ORACLE".
/// </summary>
public sealed class MainViewModel : ObservableObject
{
    private readonly RunDiscoveryService _discovery = new();
    private readonly RaceReportService _raceReport = new();
    private readonly RaceReportExcelWriter _excelWriter = new();
    private readonly IReportAppender _appender = new ExcelReportAppender();
    private readonly SettingsService _settingsService = new();
    private readonly ReportManagerSettings _settings;

    private string _selectedFolder = "";
    private bool _isBusy;
    private string _statusText = "Select a session or track folder to begin.";

    public MainViewModel()
    {
        _settings = _settingsService.Load();

        BrowseCommand = new RelayCommand(Browse, () => !IsBusy);
        RefreshCommand = new AsyncRelayCommand(DiscoverAsync, () => !IsBusy && Directory.Exists(SelectedFolder));
        BrowseTargetFileCommand = new RelayCommand(BrowseTargetFile, () => !IsBusy);
        SelectAllCommand = new RelayCommand(() => SetAllSelected(true), () => !IsBusy && Runs.Count > 0);
        DeselectAllCommand = new RelayCommand(() => SetAllSelected(false), () => !IsBusy && Runs.Count > 0);
        RaceReportCommand = new AsyncRelayCommand(RaceReportAsync, CanRaceReport);
        RunOracleCommand = new AsyncRelayCommand(RunOracleAsync, () => !IsBusy && Directory.Exists(SelectedFolder));
    }

    public ObservableCollection<RunRowViewModel> Runs { get; } = new();

    public RelayCommand BrowseCommand { get; }
    public AsyncRelayCommand RefreshCommand { get; }
    public RelayCommand BrowseTargetFileCommand { get; }
    public RelayCommand SelectAllCommand { get; }
    public RelayCommand DeselectAllCommand { get; }
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

    /// <summary>When true, the report is appended to <see cref="AppendTargetPath"/> instead of creating a new file.</summary>
    public bool AppendMode
    {
        get => _settings.AppendMode;
        set
        {
            if (_settings.AppendMode == value) return;
            _settings.AppendMode = value;
            _settingsService.Save(_settings);
            OnPropertyChanged(nameof(AppendMode));
            RaceReportCommand.RaiseCanExecuteChanged();
        }
    }

    /// <summary>Absolute path of the existing .xlsx workbook to append to (used when <see cref="AppendMode"/> is on).</summary>
    public string AppendTargetPath
    {
        get => _settings.AppendTargetPath;
        set
        {
            var newValue = value ?? "";
            if (_settings.AppendTargetPath == newValue) return;
            _settings.AppendTargetPath = newValue;
            _settingsService.Save(_settings);
            OnPropertyChanged(nameof(AppendTargetPath));
            RaceReportCommand.RaiseCanExecuteChanged();
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

    /// <summary>At least one run selected → "Race Report" is allowed.</summary>
    public bool HasSelection => Runs.Any(r => r.IsSelected);

    private bool CanRaceReport() =>
        !IsBusy && HasSelection && (!AppendMode || !string.IsNullOrWhiteSpace(AppendTargetPath));

    private void RaiseAllCanExecute()
    {
        BrowseCommand.RaiseCanExecuteChanged();
        RefreshCommand.RaiseCanExecuteChanged();
        BrowseTargetFileCommand.RaiseCanExecuteChanged();
        SelectAllCommand.RaiseCanExecuteChanged();
        DeselectAllCommand.RaiseCanExecuteChanged();
        RaceReportCommand.RaiseCanExecuteChanged();
        RunOracleCommand.RaiseCanExecuteChanged();
    }

    private void Browse()
    {
        var dialog = new OpenFolderDialog { Title = "Select a session or track folder" };
        if (Directory.Exists(SelectedFolder)) dialog.InitialDirectory = SelectedFolder;
        if (dialog.ShowDialog() == true)
        {
            SelectedFolder = dialog.FolderName;
            _ = DiscoverAsync();
        }
    }

    private void BrowseTargetFile()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select the Excel workbook to append to",
            Filter = "Excel workbook (*.xlsx)|*.xlsx",
            CheckFileExists = true,
        };
        if (!string.IsNullOrWhiteSpace(AppendTargetPath) && File.Exists(AppendTargetPath))
            dialog.InitialDirectory = Path.GetDirectoryName(AppendTargetPath);
        if (dialog.ShowDialog() == true)
            AppendTargetPath = dialog.FileName;
    }

    private async Task DiscoverAsync()
    {
        if (!Directory.Exists(SelectedFolder)) { StatusText = "The selected folder does not exist."; return; }

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
                ? "No runs found in the selected folder."
                : $"{found.Count} run(s) found - select runs and press 'Race Report'.";
        }
        catch (Exception ex)
        {
            StatusText = $"Run discovery failed: {ex.Message}";
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

    private void SetAllSelected(bool selected)
    {
        foreach (var run in Runs) run.IsSelected = selected;
        OnRunSelectionChanged();
    }

    private async Task RaceReportAsync()
    {
        var selected = Runs.Where(r => r.IsSelected).Select(r => r.Run).ToList();
        if (selected.Count == 0) return;

        if (AppendMode && (string.IsNullOrWhiteSpace(AppendTargetPath) || !File.Exists(AppendTargetPath)))
        {
            StatusText = "Append target not found - choose an existing workbook.";
            ShowError("Select an existing .xlsx workbook to append to. The chosen file was not found.");
            return;
        }

        IsBusy = true;
        try
        {
            var progress = new Progress<string>(message => StatusText = message);

            if (AppendMode)
            {
                var target = AppendTargetPath;
                var appendResult = await Task.Run(() =>
                {
                    var report = _raceReport.Generate(selected, progress);
                    return _appender.Append(report, target);
                });
                HandleAppendResult(appendResult, target);
            }
            else
            {
                var (fileName, channelCount, warnings) = await Task.Run(() =>
                {
                    var result = _raceReport.Generate(selected, progress);
                    var outputFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    var path = _excelWriter.WriteNew(result, outputFolder, autoOpen: true);
                    return (Path.GetFileName(path), result.Values.Count, result.Warnings);
                });
                var warn = warnings.Count > 0 ? $" | Notes: {string.Join("; ", warnings)}" : "";
                StatusText = $"Race Report created: {fileName} ({channelCount} channels){warn}";
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Race Report failed: {ex.Message}";
            ShowError($"The Race Report could not be completed:\n\n{ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void HandleAppendResult(AppendResult result, string targetPath)
    {
        var fileName = Path.GetFileName(targetPath);
        switch (result.Status)
        {
            case AppendStatus.Success:
                StatusText = $"Report appended to '{fileName}' " +
                             $"(row {result.AppendedRowIndex}, {result.MatchedColumns.Count} column(s) matched).";
                TryOpen(targetPath);
                break;

            case AppendStatus.PartialSuccess:
                StatusText = $"Report appended to '{fileName}' with adjustments (row {result.AppendedRowIndex}).";
                ShowWarning("The report was appended, but the target workbook did not match perfectly:\n\n"
                            + string.Join("\n", result.Warnings));
                TryOpen(targetPath);
                break;

            default: // Failed
                StatusText = $"Append failed: {result.ErrorMessage}";
                ShowError(result.ErrorMessage ?? "The report could not be appended to the selected workbook.");
                break;
        }
    }

    private async Task RunOracleAsync()
    {
        var folder = SelectedFolder;
        if (!Directory.Exists(folder)) { StatusText = "Select a valid folder first."; return; }

        IsBusy = true;
        try
        {
            var progress = new Progress<string>(message => StatusText = message);
            // Processes ALL runs in the selected track/session folder: process per run + merge per rider.
            var code = await Task.Run(() => OraclePipeline.Create().ProcessFolderAsync(folder, force: false, progress));
            StatusText = $"ORACLE finished: {code}. Press 'Refresh' to reload the updated runs.";
        }
        catch (Exception ex)
        {
            StatusText = $"ORACLE failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static void TryOpen(string path)
    {
        try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(path) { UseShellExecute = true }); }
        catch { /* opening the workbook is a convenience; ignore failures */ }
    }

    private static void ShowError(string message) =>
        System.Windows.MessageBox.Show(message, "KTM Offroad Racing Report Manager",
            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

    private static void ShowWarning(string message) =>
        System.Windows.MessageBox.Show(message, "KTM Offroad Racing Report Manager",
            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
}
