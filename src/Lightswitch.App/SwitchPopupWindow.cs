using H.NotifyIcon;
using Lightswitch.App.Controls;
using Lightswitch.App.ViewModels;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;
using Windows.Graphics;

namespace Lightswitch.App;

public sealed class SwitchPopupWindow : Window
{
    private const int PopupWidth = 160;
    private const int PopupHeight = 200;
    private const int PlateLeft = 41;
    private const int PlateTop = 35;
    private const int PlateWidth = 78;
    private const int PlateHeight = 130;
    private bool _allowClose;
    private InputNonClientPointerSource? _nonClientPointerSource;

    public SwitchPopupWindow(MainViewModel viewModel)
    {
        Title = "Lightswitch";
        var popup = new SwitchPopup
        {
            DataContext = viewModel
        };
        Content = popup;

        ResizeToDesignSize();
        ConfigureNativeDragRegions();
        AppWindow.Closing += OnClosing;

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.SetBorderAndTitleBar(hasBorder: false, hasTitleBar: false);
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
        }
    }

    public void ShowPopup()
    {
        ResizeToDesignSize();
        ConfigureNativeDragRegions();
        this.Show(disableEfficiencyMode: true);
        ResizeToDesignSize();
        ConfigureNativeDragRegions();
        Activate();
    }

    public void CloseForExit()
    {
        _allowClose = true;
        Close();
    }

    private void OnClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (_allowClose)
        {
            return;
        }

        args.Cancel = true;
        this.Hide(enableEfficiencyMode: true);
    }

    private void ResizeToDesignSize()
    {
        var hwnd = WindowNative.GetWindowHandle(this);
        var dpi = GetDpiForWindow(hwnd);
        var scale = dpi / 96.0;

        AppWindow.Resize(new SizeInt32(
            (int)Math.Round(PopupWidth * scale),
            (int)Math.Round(PopupHeight * scale)));
    }

    private void ConfigureNativeDragRegions()
    {
        _nonClientPointerSource ??= InputNonClientPointerSource.GetForWindowId(AppWindow.Id);

        var hwnd = WindowNative.GetWindowHandle(this);
        var dpi = GetDpiForWindow(hwnd);
        var scale = dpi / 96.0;

        var plateLeft = Scale(PlateLeft, scale);
        var plateTop = Scale(PlateTop, scale);
        var plateWidth = Scale(PlateWidth, scale);
        var plateHeight = Scale(PlateHeight, scale);
        var popupWidth = Scale(PopupWidth, scale);
        var popupHeight = Scale(PopupHeight, scale);

        var captionRects = new[]
        {
            new RectInt32(0, 0, popupWidth, plateTop),
            new RectInt32(0, plateTop + plateHeight, popupWidth, popupHeight - plateTop - plateHeight),
            new RectInt32(0, plateTop, plateLeft, plateHeight),
            new RectInt32(plateLeft + plateWidth, plateTop, popupWidth - plateLeft - plateWidth, plateHeight)
        };

        _nonClientPointerSource.SetRegionRects(NonClientRegionKind.Caption, captionRects);
    }

    private static int Scale(int value, double scale) => (int)Math.Round(value * scale);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(IntPtr hwnd);
}
