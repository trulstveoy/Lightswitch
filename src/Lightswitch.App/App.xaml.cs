using Lightswitch.App.Infrastructure;
using Lightswitch.App.ViewModels;
using Lightswitch.Device;
using Microsoft.UI.Xaml;

namespace Lightswitch.App;

public partial class App : Application
{
    private SingleInstanceGuard? _singleInstanceGuard;
    private LitraService? _litraService;
    private MainWindow? _window;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _singleInstanceGuard = new SingleInstanceGuard();
        if (!_singleInstanceGuard.IsFirstInstance)
        {
            Exit();
            return;
        }

        _litraService = new LitraService();
        var settingsStore = new JsonSettingsStore();
        var startupRegistration = new StartupRegistrationService();
        var viewModel = new MainViewModel(_litraService, settingsStore, startupRegistration);

        _window = new MainWindow(viewModel);
        _window.Activate();
        _window.HideToTray();

        _ = viewModel.InitializeAsync();
    }
}
