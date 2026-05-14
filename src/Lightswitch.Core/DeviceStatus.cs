namespace Lightswitch.Core;

public sealed record DeviceStatus(
    DeviceConnectionState ConnectionState,
    string Message,
    string? DeviceName = null)
{
    public static DeviceStatus Unknown { get; } = new(DeviceConnectionState.Unknown, "Device status unknown.");

    public static DeviceStatus Disconnected(string message = "Logitech Litra Glow is not connected.") =>
        new(DeviceConnectionState.Disconnected, message);

    public static DeviceStatus Connected(string deviceName) =>
        new(DeviceConnectionState.Connected, "Logitech Litra Glow is connected.", deviceName);

    public static DeviceStatus Error(string message) =>
        new(DeviceConnectionState.Error, message);
}
