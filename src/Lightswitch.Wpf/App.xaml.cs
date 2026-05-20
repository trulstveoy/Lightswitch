using Lightswitch.Device;
using Lightswitch.Wpf.Infrastructure;
using Lightswitch.Wpf.ViewModels;
using System.Windows;

namespace Lightswitch.Wpf;

public partial class App : System.Windows.Application
{
    private SingleInstanceGuard? _singleInstanceGuard;
    private LitraService? _litraService;
    private TrayAppController? _trayController;
    private MainViewModel? _viewModel;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _singleInstanceGuard = new SingleInstanceGuard();
        if (!_singleInstanceGuard.IsFirstInstance)
        {
            Shutdown();
            return;
        }

        _litraService = new LitraService();
        var settingsStore = new JsonSettingsStore();
        var startupRegistration = new StartupRegistrationService();
        _viewModel = new MainViewModel(_litraService, settingsStore, startupRegistration);
        _trayController = new TrayAppController(_viewModel, ShutdownApplication);

        try
        {
            await _viewModel.InitializeAsync();
        }
        catch (Exception ex)
        {
            _viewModel.ReportError(ex.Message);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _trayController?.Dispose();
        _litraService?.Dispose();
        _singleInstanceGuard?.Dispose();
        base.OnExit(e);
    }

    private void ShutdownApplication() => Shutdown();
}
