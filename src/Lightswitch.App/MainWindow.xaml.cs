using System.Windows.Input;
using H.NotifyIcon;
using Lightswitch.App.Infrastructure;
using Lightswitch.App.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Lightswitch.App;

public sealed partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private bool _isExitRequested;

    public MainWindow(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        ShowWindowCommand = new RelayCommand(ShowFromTray);

        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        AppWindow.SetIcon("Assets/AppIcon.ico");
        AppWindow.Closing += AppWindow_Closing;

        TrayIcon.ForceCreate();
        RootFrame.Navigate(typeof(MainPage), _viewModel);
    }

    public ICommand ShowWindowCommand { get; }

    private void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
    {
        if (_isExitRequested)
        {
            return;
        }

        args.Cancel = true;
        this.Hide();
    }

    private void ShowFromTray()
    {
        this.Show();
        Activate();
    }

    private void OpenMenuItem_Click(object sender, RoutedEventArgs e) => ShowFromTray();

    private void TogglePowerMenuItem_Click(object sender, RoutedEventArgs e) =>
        _viewModel.TogglePowerCommand.Execute(null);

    private void Brightness25MenuItem_Click(object sender, RoutedEventArgs e) =>
        _viewModel.SetBrightnessCommand.Execute(25);

    private void Brightness50MenuItem_Click(object sender, RoutedEventArgs e) =>
        _viewModel.SetBrightnessCommand.Execute(50);

    private void Brightness100MenuItem_Click(object sender, RoutedEventArgs e) =>
        _viewModel.SetBrightnessCommand.Execute(100);

    private void WarmMenuItem_Click(object sender, RoutedEventArgs e) =>
        _viewModel.SetTemperatureCommand.Execute(3000);

    private void NeutralMenuItem_Click(object sender, RoutedEventArgs e) =>
        _viewModel.SetTemperatureCommand.Execute(4000);

    private void CoolMenuItem_Click(object sender, RoutedEventArgs e) =>
        _viewModel.SetTemperatureCommand.Execute(5600);

    private void StartWithWindowsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleMenuFlyoutItem item)
        {
            _viewModel.StartWithWindows = item.IsChecked;
        }
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        _isExitRequested = true;
        TrayIcon.Dispose();
        Close();
    }
}
