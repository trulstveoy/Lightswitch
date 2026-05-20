using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Lightswitch.Wpf.ViewModels;

namespace Lightswitch.Wpf;

public partial class SwitchWindow : Window
{
    private static readonly Duration ToggleDuration = new(TimeSpan.FromMilliseconds(180));
    private readonly MainViewModel _viewModel;
    private readonly Action _showSettings;
    private readonly Action _exit;
    private bool _allowClose;

    public SwitchWindow(MainViewModel viewModel, Action showSettings, Action exit)
    {
        _viewModel = viewModel;
        _showSettings = showSettings;
        _exit = exit;
        InitializeComponent();
        DataContext = viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        ApplyState(animate: false);
    }

    public void ShowPopup()
    {
        ApplyState(animate: false);
        Show();
        Activate();
    }

    public void CloseForExit()
    {
        _allowClose = true;
        Close();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (_allowClose)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            base.OnClosing(e);
            return;
        }

        e.Cancel = true;
        Hide();
    }

    private void Root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (IsInsidePlate(e.OriginalSource as DependencyObject))
        {
            return;
        }

        try
        {
            DragMove();
        }
        catch (InvalidOperationException)
        {
            // WPF throws when the mouse button is released before DragMove starts.
        }
    }

    private void Plate_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        _viewModel.TogglePowerCommand.Execute(null);
    }

    private void SwitchContextMenu_Opened(object sender, RoutedEventArgs e) =>
        StartWithWindowsMenuItem.IsChecked = _viewModel.StartWithWindows;

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

    private void WarmTemperatureMenuItem_Click(object sender, RoutedEventArgs e) =>
        _viewModel.SetTemperatureCommand.Execute(3000);

    private void NeutralTemperatureMenuItem_Click(object sender, RoutedEventArgs e) =>
        _viewModel.SetTemperatureCommand.Execute(4000);

    private void CoolTemperatureMenuItem_Click(object sender, RoutedEventArgs e) =>
        _viewModel.SetTemperatureCommand.Execute(5600);

    private void StartWithWindowsMenuItem_Click(object sender, RoutedEventArgs e) =>
        _viewModel.StartWithWindows = StartWithWindowsMenuItem.IsChecked;

    private void SettingsMenuItem_Click(object sender, RoutedEventArgs e) =>
        _showSettings();

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e) =>
        _exit();

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.IsOn))
        {
            Dispatcher.Invoke(() => ApplyState(animate: true));
        }
    }

    private void ApplyState(bool animate)
    {
        var isOn = _viewModel.IsOn;

        Root.Background = Brush(isOn ? "#E8DFC0" : "#5A5650");
        Wall.Background = Root.Background;
        PlateFill.Background = Brush(isOn ? "#F7F2E2" : "#2E2C28");
        PlateBorder.BorderBrush = Brush(isOn ? "#CCC4A0" : "#1A1916");
        Track.Background = Brush(isOn ? "#E0D8C0" : "#1A1916");
        Track.BorderBrush = Brush(isOn ? "#BBB090" : "#0A0A08");
        Arm.Background = Brush(isOn ? "#FFF8E8" : "#222018");
        Arm.BorderBrush = Brush(isOn ? "#D0C8A8" : "#0E0D0A");
        ArmGrip.Background = Brush(isOn ? "#B8B098" : "#3A3830");

        SetScrewBrushes(isOn);
        if (animate)
        {
            AnimateArm(isOn ? 0 : 32);
            return;
        }

        ArmTransform.Y = isOn ? 0 : 32;
    }

    private void SetScrewBrushes(bool isOn)
    {
        var disc = Brush(isOn ? "#C0B898" : "#3A3830");
        var stroke = Brush(isOn ? "#9A9080" : "#222018");
        var slot = Brush(isOn ? "#8A8270" : "#111008");

        ScrewTopDisc.Fill = disc;
        ScrewTopDisc.Stroke = stroke;
        ScrewBottomDisc.Fill = disc;
        ScrewBottomDisc.Stroke = stroke;
        ScrewTopSlot.Fill = slot;
        ScrewBottomSlot.Fill = slot;
    }

    private void AnimateArm(double target)
    {
        var animation = new DoubleAnimation
        {
            To = target,
            Duration = ToggleDuration,
            EasingFunction = new BackEase
            {
                Amplitude = 0.35,
                EasingMode = EasingMode.EaseOut
            }
        };

        ArmTransform.BeginAnimation(TranslateTransform.YProperty, animation);
    }

    private bool IsInsidePlate(DependencyObject? source)
    {
        while (source is not null)
        {
            if (ReferenceEquals(source, Plate))
            {
                return true;
            }

            source = VisualTreeHelper.GetParent(source);
        }

        return false;
    }

    private static SolidColorBrush Brush(string hex) =>
        new(System.Windows.Media.Color.FromRgb(Convert.ToByte(hex[1..3], 16), Convert.ToByte(hex[3..5], 16), Convert.ToByte(hex[5..7], 16)));
}
