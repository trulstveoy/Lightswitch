using System.ComponentModel;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Lightswitch.Wpf.ViewModels;
using Forms = System.Windows.Forms;
using DrawingBitmap = System.Drawing.Bitmap;
using DrawingColor = System.Drawing.Color;
using DrawingFont = System.Drawing.Font;
using DrawingFontStyle = System.Drawing.FontStyle;
using DrawingGraphics = System.Drawing.Graphics;
using DrawingGraphicsUnit = System.Drawing.GraphicsUnit;
using DrawingIcon = System.Drawing.Icon;
using DrawingRectangle = System.Drawing.Rectangle;
using DrawingStringFormat = System.Drawing.StringFormat;
using DrawingStringAlignment = System.Drawing.StringAlignment;
using DrawingSolidBrush = System.Drawing.SolidBrush;

namespace Lightswitch.Wpf;

public sealed class TrayAppController : IDisposable
{
    private readonly MainViewModel _viewModel;
    private readonly Action _exit;
    private readonly Dispatcher _dispatcher;
    private readonly Forms.NotifyIcon _notifyIcon;
    private readonly Forms.ContextMenuStrip _contextMenu;
    private readonly Forms.ToolStripMenuItem _startWithWindowsItem;
    private MainWindow? _settingsWindow;
    private SwitchWindow? _switchWindow;
    private bool _disposed;

    public TrayAppController(MainViewModel viewModel, Action exit)
    {
        _viewModel = viewModel;
        _exit = exit;
        _dispatcher = System.Windows.Application.Current.Dispatcher;

        _startWithWindowsItem = new Forms.ToolStripMenuItem("Start with Windows")
        {
            CheckOnClick = true
        };
        _startWithWindowsItem.CheckedChanged += StartWithWindowsItem_CheckedChanged;

        _contextMenu = BuildContextMenu();
        _contextMenu.Opening += ContextMenu_Opening;

        _notifyIcon = new Forms.NotifyIcon
        {
            Icon = CreateTrayIcon(),
            Text = "Lightswitch",
            Visible = true,
            ContextMenuStrip = _contextMenu
        };
        _notifyIcon.MouseUp += NotifyIcon_MouseUp;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _notifyIcon.Visible = false;
        _notifyIcon.MouseUp -= NotifyIcon_MouseUp;
        _notifyIcon.Icon?.Dispose();
        _notifyIcon.Dispose();
        _contextMenu.Dispose();
        _switchWindow?.CloseForExit();
        _settingsWindow?.CloseForExit();
    }

    private Forms.ContextMenuStrip BuildContextMenu()
    {
        var menu = new Forms.ContextMenuStrip();

        menu.Items.Add("Skru av/pa", null, (_, _) => RunOnUi(() => _viewModel.TogglePowerCommand.Execute(null)));

        var brightness = new Forms.ToolStripMenuItem("Brightness");
        brightness.DropDownItems.Add("25%", null, (_, _) => RunOnUi(() => _viewModel.SetBrightnessCommand.Execute(25)));
        brightness.DropDownItems.Add("50%", null, (_, _) => RunOnUi(() => _viewModel.SetBrightnessCommand.Execute(50)));
        brightness.DropDownItems.Add("75%", null, (_, _) => RunOnUi(() => _viewModel.SetBrightnessCommand.Execute(75)));
        brightness.DropDownItems.Add("100%", null, (_, _) => RunOnUi(() => _viewModel.SetBrightnessCommand.Execute(100)));
        menu.Items.Add(brightness);

        var temperature = new Forms.ToolStripMenuItem("Color temperature");
        temperature.DropDownItems.Add("Warm", null, (_, _) => RunOnUi(() => _viewModel.SetTemperatureCommand.Execute(3000)));
        temperature.DropDownItems.Add("Neutral", null, (_, _) => RunOnUi(() => _viewModel.SetTemperatureCommand.Execute(4000)));
        temperature.DropDownItems.Add("Cool", null, (_, _) => RunOnUi(() => _viewModel.SetTemperatureCommand.Execute(5600)));
        menu.Items.Add(temperature);

        menu.Items.Add(new Forms.ToolStripSeparator());
        menu.Items.Add(_startWithWindowsItem);
        menu.Items.Add("Innstillinger", null, (_, _) => RunOnUi(ShowSettingsWindow));
        menu.Items.Add(new Forms.ToolStripSeparator());
        menu.Items.Add("Avslutt", null, (_, _) => RunOnUi(_exit));

        return menu;
    }

    private void NotifyIcon_MouseUp(object? sender, Forms.MouseEventArgs e)
    {
        if (e.Button == Forms.MouseButtons.Left)
        {
            RunOnUi(ShowSwitchWindow);
        }
    }

    private void ShowSwitchWindow()
    {
        _switchWindow ??= new SwitchWindow(_viewModel);
        PositionSwitchWindow(_switchWindow);
        _switchWindow.ShowPopup();
    }

    private void ShowSettingsWindow()
    {
        _settingsWindow ??= new MainWindow(_viewModel);
        if (!_settingsWindow.IsVisible)
        {
            _settingsWindow.Show();
        }

        if (_settingsWindow.WindowState == WindowState.Minimized)
        {
            _settingsWindow.WindowState = WindowState.Normal;
        }

        _settingsWindow.Activate();
    }

    private void ContextMenu_Opening(object? sender, CancelEventArgs e)
    {
        _startWithWindowsItem.CheckedChanged -= StartWithWindowsItem_CheckedChanged;
        _startWithWindowsItem.Checked = _dispatcher.Invoke(() => _viewModel.StartWithWindows);
        _startWithWindowsItem.CheckedChanged += StartWithWindowsItem_CheckedChanged;
    }

    private void StartWithWindowsItem_CheckedChanged(object? sender, EventArgs e) =>
        RunOnUi(() => _viewModel.StartWithWindows = _startWithWindowsItem.Checked);

    private void RunOnUi(Action action)
    {
        if (_dispatcher.CheckAccess())
        {
            action();
            return;
        }

        _dispatcher.Invoke(action);
    }

    private static void PositionSwitchWindow(Window window)
    {
        var cursor = Forms.Cursor.Position;
        var point = new System.Windows.Point(cursor.X, cursor.Y);
        var helper = new WindowInteropHelper(window);
        var hwnd = helper.EnsureHandle();
        var source = HwndSource.FromHwnd(hwnd);
        if (source?.CompositionTarget is not null)
        {
            point = source.CompositionTarget.TransformFromDevice.Transform(point);
        }

        var workArea = SystemParameters.WorkArea;
        var left = Math.Clamp(point.X - window.Width / 2, workArea.Left, workArea.Right - window.Width);
        var top = Math.Clamp(point.Y - window.Height - 8, workArea.Top, workArea.Bottom - window.Height);

        window.Left = left;
        window.Top = top;
    }

    private static DrawingIcon CreateTrayIcon()
    {
        using var bitmap = new DrawingBitmap(44, 44);
        using var graphics = DrawingGraphics.FromImage(bitmap);
        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        graphics.Clear(DrawingColor.Black);

        using var font = new DrawingFont("Segoe UI", 30, DrawingFontStyle.Bold, DrawingGraphicsUnit.Pixel);
        using var foreground = new DrawingSolidBrush(DrawingColor.White);
        using var format = new DrawingStringFormat
        {
            Alignment = DrawingStringAlignment.Center,
            LineAlignment = DrawingStringAlignment.Center
        };

        graphics.DrawString("L", font, foreground, new DrawingRectangle(0, -2, 44, 44), format);

        var handle = bitmap.GetHicon();
        try
        {
            using var icon = DrawingIcon.FromHandle(handle);
            return (DrawingIcon)icon.Clone();
        }
        finally
        {
            DestroyIcon(handle);
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);
}
