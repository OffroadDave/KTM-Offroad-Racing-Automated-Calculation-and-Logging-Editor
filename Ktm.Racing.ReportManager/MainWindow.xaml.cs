using System.Windows;
using Ktm.Racing.RaceReport.ViewModels;

namespace Ktm.Racing.RaceReport;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
