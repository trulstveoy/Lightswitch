using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Lightswitch.Core;
using Lightswitch.Wpf.Infrastructure;

namespace Lightswitch.Wpf.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly ILitraService _litraService;
    private readonly ISettingsStore _settingsStore;
    private readonly StartupRegistrationService _startupRegistrationService;
    private LightSettings _settings = new();
    private bool _isBusy;
    private bool _isOn = LightState.Default.IsOn;
    private int _brightness = LightState.Default.Brightness;
    private int _temperatureKelvin = LightState.Default.TemperatureKelvin;
    private bool _startWithWindows;
    private string _statusText = DeviceStatus.Unknown.Message;

    public MainViewModel(
        ILitraService litraService,
        ISettingsStore settingsStore,
        StartupRegistrationService startupRegistrationService)
    {
        _litraService = litraService;
        _settingsStore = settingsStore;
        _startupRegistrationService = startupRegistrationService;
        _litraService.StatusChanged += OnDeviceStatusChanged;

        TogglePowerCommand = new RelayCommand(() => IsOn = !IsOn);
        SetBrightnessCommand = new RelayCommand(parameter => SetBrightness(Convert.ToInt32(parameter)));
        SetTemperatureCommand = new RelayCommand(parameter => SetTemperature(Convert.ToInt32(parameter)));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public RelayCommand TogglePowerCommand { get; }

    public RelayCommand SetBrightnessCommand { get; }

    public RelayCommand SetTemperatureCommand { get; }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetField(ref _isBusy, value);
    }

    public bool IsOn
    {
        get => _isOn;
        set
        {
            if (SetField(ref _isOn, value))
            {
                _ = ApplyCurrentStateAsync();
            }
        }
    }

    public int Brightness
    {
        get => _brightness;
        set
        {
            var normalized = LightLimits.ClampBrightness(value);
            if (SetField(ref _brightness, normalized))
            {
                _ = ApplyCurrentStateAsync();
            }
        }
    }

    public int TemperatureKelvin
    {
        get => _temperatureKelvin;
        set
        {
            var normalized = LightLimits.ClampTemperatureKelvin(value);
            if (SetField(ref _temperatureKelvin, normalized))
            {
                _ = ApplyCurrentStateAsync();
            }
        }
    }

    public bool StartWithWindows
    {
        get => _startWithWindows;
        set
        {
            if (!SetField(ref _startWithWindows, value))
            {
                return;
            }

            _startupRegistrationService.SetEnabled(value);
            _settings = _settings with { StartWithWindows = value };
            _ = _settingsStore.SaveAsync(_settings);
        }
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetField(ref _statusText, value);
    }

    public void ReportError(string message) => StatusText = message;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try
        {
            _settings = await _settingsStore.LoadAsync(cancellationToken);
            var state = _settings.LastState.Normalize();

            _isOn = state.IsOn;
            _brightness = state.Brightness;
            _temperatureKelvin = state.TemperatureKelvin;
            _startWithWindows = _startupRegistrationService.IsEnabled();

            OnPropertyChanged(nameof(IsOn));
            OnPropertyChanged(nameof(Brightness));
            OnPropertyChanged(nameof(TemperatureKelvin));
            OnPropertyChanged(nameof(StartWithWindows));

            var status = await _litraService.RefreshAsync(cancellationToken);
            StatusText = status.Message;
            await _litraService.ApplyAsync(state, cancellationToken);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void SetBrightness(int brightness) => Brightness = brightness;

    private void SetTemperature(int temperatureKelvin) => TemperatureKelvin = temperatureKelvin;

    private async Task ApplyCurrentStateAsync()
    {
        var state = new LightState
        {
            IsOn = IsOn,
            Brightness = Brightness,
            TemperatureKelvin = TemperatureKelvin
        }.Normalize();

        _settings = _settings with
        {
            LastState = state,
            StartWithWindows = StartWithWindows
        };

        try
        {
            await _settingsStore.SaveAsync(_settings);
            await _litraService.ApplyAsync(state);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            StatusText = ex.Message;
        }
    }

    private void OnDeviceStatusChanged(object? sender, DeviceStatus status)
    {
        StatusText = status.DeviceName is null
            ? status.Message
            : $"{status.Message} ({status.DeviceName})";
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
