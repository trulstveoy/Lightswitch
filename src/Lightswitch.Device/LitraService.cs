using HidSharp;
using Lightswitch.Core;

namespace Lightswitch.Device;

public sealed class LitraService : ILitraService, IDisposable
{
    private readonly DeviceList _deviceList;
    private readonly SemaphoreSlim _sync = new(1, 1);
    private HidDevice? _selectedDevice;
    private bool _disposed;

    public LitraService()
        : this(DeviceList.Local)
    {
    }

    internal LitraService(DeviceList deviceList)
    {
        _deviceList = deviceList;
        _deviceList.Changed += OnDeviceListChanged;
    }

    public event EventHandler<DeviceStatus>? StatusChanged;

    public DeviceStatus Status { get; private set; } = DeviceStatus.Unknown;

    public LightState DesiredState { get; private set; } = LightState.Default;

    public async Task<DeviceStatus> RefreshAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        await _sync.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var status = await Task.Run(RefreshDevice, cancellationToken).ConfigureAwait(false);
            SetStatus(status);
            return Status;
        }
        finally
        {
            _sync.Release();
        }
    }

    public async Task ApplyAsync(LightState state, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        DesiredState = state.Normalize();

        await _sync.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var status = RefreshDevice();
            SetStatus(status);

            if (status.ConnectionState != DeviceConnectionState.Connected || _selectedDevice is null)
            {
                return;
            }

            await Task.Run(() => ApplyDesiredState(_selectedDevice, DesiredState), cancellationToken).ConfigureAwait(false);
            SetStatus(DeviceStatus.Connected(GetDisplayName(_selectedDevice)));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or TimeoutException or InvalidOperationException)
        {
            _selectedDevice = null;
            SetStatus(DeviceStatus.Error(ex.Message));
        }
        finally
        {
            _sync.Release();
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _deviceList.Changed -= OnDeviceListChanged;
        _sync.Dispose();
        _disposed = true;
    }

    private DeviceStatus RefreshDevice()
    {
        try
        {
            _selectedDevice = FindLitraGlowDevice();

            return _selectedDevice is null
                ? DeviceStatus.Disconnected()
                : DeviceStatus.Connected(GetDisplayName(_selectedDevice));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            _selectedDevice = null;
            return DeviceStatus.Error(ex.Message);
        }
    }

    private HidDevice? FindLitraGlowDevice()
    {
        var exactMatches = _deviceList
            .GetHidDevices(vendorID: LitraDeviceIds.LogitechVendorId, productID: LitraDeviceIds.LitraGlowProductId)
            .Where(HasWritableLitraReport)
            .ToList();

        if (exactMatches.Count > 0)
        {
            return exactMatches
                .OrderByDescending(device => SafeRead(device.GetFileSystemName)?.Contains("&col02#", StringComparison.OrdinalIgnoreCase) == true)
                .First();
        }

        return _deviceList
            .GetHidDevices(vendorID: LitraDeviceIds.LogitechVendorId)
            .Where(IsLikelyLitraGlow)
            .FirstOrDefault(HasWritableLitraReport);
    }

    private static bool HasWritableLitraReport(HidDevice device) =>
        SafeReadInt(device.GetMaxOutputReportLength) >= LitraProtocol.OutputReportLength;

    private static void ApplyDesiredState(HidDevice device, LightState state)
    {
        WriteReport(device, LitraProtocol.BuildPowerReport(state.IsOn, GetReportLength(device)));

        if (!state.IsOn)
        {
            return;
        }

        WriteReport(device, LitraProtocol.BuildBrightnessReport(state.Brightness, GetReportLength(device)));
        WriteReport(device, LitraProtocol.BuildTemperatureReport(state.TemperatureKelvin, GetReportLength(device)));
    }

    private static void WriteReport(HidDevice device, byte[] report)
    {
        if (!device.TryOpen(out var stream))
        {
            throw new IOException("Unable to open Logitech Litra Glow HID interface.");
        }

        using (stream)
        {
            stream.WriteTimeout = 1000;
            stream.Write(report);
        }
    }

    private static int GetReportLength(HidDevice device) =>
        Math.Max(device.GetMaxOutputReportLength(), LitraProtocol.OutputReportLength);

    private static bool IsLikelyLitraGlow(HidDevice device)
    {
        var productName = SafeRead(device.GetProductName);
        var friendlyName = SafeRead(device.GetFriendlyName);
        var path = SafeRead(device.GetFileSystemName);

        return ContainsLitra(productName)
            || ContainsLitra(friendlyName)
            || path?.Contains("pid_c900", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static string GetDisplayName(HidDevice device)
    {
        var productName = SafeRead(device.GetProductName);
        if (!string.IsNullOrWhiteSpace(productName))
        {
            return productName;
        }

        var friendlyName = SafeRead(device.GetFriendlyName);
        if (!string.IsNullOrWhiteSpace(friendlyName))
        {
            return friendlyName;
        }

        return device.ProductID == LitraDeviceIds.LitraGlowProductId
            ? "Logitech Litra Glow"
            : "Logitech HID device";
    }

    private static bool ContainsLitra(string? value) =>
        value?.Contains("Litra", StringComparison.OrdinalIgnoreCase) == true;

    private static string? SafeRead(Func<string?> read)
    {
        try
        {
            return read();
        }
        catch
        {
            return null;
        }
    }

    private static int SafeReadInt(Func<int> read)
    {
        try
        {
            return read();
        }
        catch
        {
            return 0;
        }
    }

    private void OnDeviceListChanged(object? sender, DeviceListChangedEventArgs e)
    {
        _ = RefreshAsync();
    }

    private void SetStatus(DeviceStatus status)
    {
        if (Status == status)
        {
            return;
        }

        Status = status;
        StatusChanged?.Invoke(this, status);
    }
}
