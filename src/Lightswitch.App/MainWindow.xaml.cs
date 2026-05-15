using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Input;
using H.NotifyIcon;
using Lightswitch.App.Infrastructure;
using Lightswitch.App.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DrawingBrush = System.Drawing.Brush;
using DrawingColor = System.Drawing.Color;
using DrawingPen = System.Drawing.Pen;
using DrawingPoint = System.Drawing.Point;
using DrawingRectangle = System.Drawing.Rectangle;
using DrawingSize = System.Drawing.Size;

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
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.Clear(DrawingColor.Transparent);

        using var background = new SolidBrush(DrawingColor.FromArgb(255, 26, 26, 26));
        using var backgroundBorder = new DrawingPen(DrawingColor.FromArgb(255, 51, 51, 51), 1);
        FillRoundedRectangle(graphics, background, new DrawingRectangle(0, 0, 43, 43), 9);
        DrawRoundedRectangle(graphics, backgroundBorder, new DrawingRectangle(0, 0, 43, 43), 9);

        using var plateBrush = new SolidBrush(DrawingColor.FromArgb(255, 58, 56, 48));
        using var platePen = new DrawingPen(DrawingColor.FromArgb(255, 85, 80, 64), 1);
        FillRoundedRectangle(graphics, plateBrush, new DrawingRectangle(14, 8, 16, 28), 3);
        DrawRoundedRectangle(graphics, platePen, new DrawingRectangle(14, 8, 16, 28), 3);

        var armY = isOn ? 10 : 22;
        using var armBrush = new SolidBrush(isOn ? DrawingColor.FromArgb(255, 255, 248, 232) : DrawingColor.FromArgb(255, 34, 32, 24));
        using var armPen = new DrawingPen(isOn ? DrawingColor.FromArgb(255, 192, 184, 144) : DrawingColor.FromArgb(255, 17, 16, 8), 1);
        FillRoundedRectangle(graphics, armBrush, new DrawingRectangle(16, armY, 12, 12), 2);
        DrawRoundedRectangle(graphics, armPen, new DrawingRectangle(16, armY, 12, 12), 2);

        using var grip = new SolidBrush(isOn ? DrawingColor.FromArgb(255, 184, 176, 152) : DrawingColor.FromArgb(255, 58, 56, 48));
        FillRoundedRectangle(graphics, grip, new DrawingRectangle(19, armY + 5, 6, 2), 1);

        return Icon.FromHandle(bitmap.GetHicon());
    }

    private static void FillRoundedRectangle(Graphics graphics, DrawingBrush brush, DrawingRectangle bounds, int radius)
    {
        using var path = RoundedRectangle(bounds, radius);
        graphics.FillPath(brush, path);
    }

    private static void DrawRoundedRectangle(Graphics graphics, DrawingPen pen, DrawingRectangle bounds, int radius)
    {
        using var path = RoundedRectangle(bounds, radius);
        graphics.DrawPath(pen, path);
    }

    private static GraphicsPath RoundedRectangle(DrawingRectangle bounds, int radius)
    {
        var diameter = radius * 2;
        var size = new DrawingSize(diameter, diameter);
        var arc = new DrawingRectangle(bounds.Location, size);
        var path = new GraphicsPath();

        path.AddArc(arc, 180, 90);
        arc.X = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);
        arc.Y = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);
        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);
        path.CloseFigure();

        return path;
    }
}
