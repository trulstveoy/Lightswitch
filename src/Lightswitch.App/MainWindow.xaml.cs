using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Input;
using H.NotifyIcon;
using Lightswitch.App.Infrastructure;
using Lightswitch.App.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DrawingColor = System.Drawing.Color;
using DrawingRectangle = System.Drawing.Rectangle;

namespace Lightswitch.App;

public sealed partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private readonly SwitchPopupWindow _switchPopupWindow;
    private Icon? _trayIcon;
    private bool _isExitRequested;

    public MainWindow(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        _switchPopupWindow = new SwitchPopupWindow(_viewModel);
        ShowWindowCommand = new RelayCommand(ShowFromTray);
        ShowSwitchPopupCommand = new RelayCommand(ShowSwitchPopup);

        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        AppWindow.SetIcon("Assets/AppIcon.ico");
        AppWindow.Closing += AppWindow_Closing;

        TrayIcon.ForceCreate();
        UpdateTrayIcon();
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        RootFrame.Navigate(typeof(MainPage), _viewModel);
    }

    public ICommand ShowWindowCommand { get; }

    public ICommand ShowSwitchPopupCommand { get; }

    public MainViewModel ViewModel => _viewModel;

    public void HideToTray() => this.Hide();

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

    private void ShowSwitchPopup() => _switchPopupWindow.ShowPopup();

    private void OpenMenuItem_Click(object sender, RoutedEventArgs e) => ShowFromTray();

    private void TogglePowerMenuItem_Click(object sender, RoutedEventArgs e) =>
        _viewModel.TogglePowerCommand.Execute(null);

    private void Brightness25MenuItem_Click(object sender, RoutedEventArgs e) =>
        _viewModel.SetBrightnessCommand.Execute(25);

    private void Brightness50MenuItem_Click(object sender, RoutedEventArgs e) =>
        _viewModel.SetBrightnessCommand.Execute(50);

    private void Brightness75MenuItem_Click(object sender, RoutedEventArgs e) =>
        _viewModel.SetBrightnessCommand.Execute(75);

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
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        _switchPopupWindow.CloseForExit();
        TrayIcon.Dispose();
        _trayIcon?.Dispose();
        Close();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.IsOn))
        {
            UpdateTrayIcon();
        }
    }

    private void UpdateTrayIcon()
    {
        var previousIcon = _trayIcon;
        _trayIcon = CreateSwitchIcon(_viewModel.IsOn);
        TrayIcon.UpdateIcon(_trayIcon);
        previousIcon?.Dispose();
    }

    private static Icon CreateSwitchIcon(bool isOn)
    {
        using var bitmap = new Bitmap(44, 44);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        graphics.Clear(DrawingColor.Black);

        using var font = new Font("Segoe UI", 30, FontStyle.Bold, GraphicsUnit.Pixel);
        using var foreground = new SolidBrush(DrawingColor.White);
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        graphics.DrawString("L", font, foreground, new DrawingRectangle(0, -2, 44, 44), format);

        return Icon.FromHandle(bitmap.GetHicon());
    }
}
