using System.ComponentModel;
using Lightswitch.App.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

namespace Lightswitch.App.Controls;

public sealed partial class SwitchPopup : UserControl
{
    private static readonly TimeSpan ToggleDuration = TimeSpan.FromMilliseconds(180);
    private MainViewModel? _viewModel;

    public SwitchPopup()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _viewModel = DataContext as MainViewModel;
        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        ApplyState(animate: false);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.IsOn))
        {
            DispatcherQueue.TryEnqueue(() => ApplyState(animate: true));
        }
    }

    private void Switch_Tapped(object sender, TappedRoutedEventArgs e)
    {
        e.Handled = true;
        _viewModel ??= DataContext as MainViewModel;
        _viewModel?.TogglePowerCommand.Execute(null);
    }

    private void ApplyState(bool animate)
    {
        var isOn = _viewModel?.IsOn ?? true;

        Wall.Background = Brush(isOn ? "#E8DFC0" : "#5A5650");
        Plate.Background = Brush(isOn ? "#F7F2E2" : "#2E2C28");
        Plate.BorderBrush = Brush(isOn ? "#CCC4A0" : "#1A1916");
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
        var storyboard = new Storyboard();
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

        Storyboard.SetTarget(animation, ArmTransform);
        Storyboard.SetTargetProperty(animation, nameof(TranslateTransform.Y));
        storyboard.Children.Add(animation);
        storyboard.Begin();
    }

    private static SolidColorBrush Brush(string hex) =>
        new(ColorHelper.FromArgb(255, Convert.ToByte(hex[1..3], 16), Convert.ToByte(hex[3..5], 16), Convert.ToByte(hex[5..7], 16)));
}
