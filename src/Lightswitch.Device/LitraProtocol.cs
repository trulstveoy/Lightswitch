namespace Lightswitch.Device;

using Lightswitch.Core;

public static class LitraProtocol
{
    public const int OutputReportLength = 20;

    private static readonly byte[] PowerOnPayload = [0x11, 0xFF, 0x04, 0x1C, 0x01];
    private static readonly byte[] PowerOffPayload = [0x11, 0xFF, 0x04, 0x1C, 0x00];

    public static byte[] BuildPowerReport(bool isOn, int reportLength = OutputReportLength)
    {
        var payload = isOn ? PowerOnPayload : PowerOffPayload;
        return BuildReport(payload, reportLength);
    }

    public static byte[] BuildBrightnessReport(int brightness, int reportLength = OutputReportLength)
    {
        var normalized = LightLimits.ClampBrightness(brightness);
        var deviceBrightness = MapBrightness(normalized);
        byte[] payload = [0x11, 0xFF, 0x04, 0x4C, 0x00, deviceBrightness];

        return BuildReport(payload, reportLength);
    }

    public static byte[] BuildTemperatureReport(int temperatureKelvin, int reportLength = OutputReportLength)
    {
        var normalized = LightLimits.ClampTemperatureKelvin(temperatureKelvin);
        byte[] payload =
        [
            0x11,
            0xFF,
            0x04,
            0x9C,
            (byte)(normalized / 256),
            (byte)(normalized % 256)
        ];

        return BuildReport(payload, reportLength);
    }

    private static byte MapBrightness(int brightness)
    {
        const int deviceMin = 20;
        const int deviceMax = 250;
        var mapped = deviceMin + (brightness / 100.0 * (deviceMax - deviceMin));

        return (byte)Math.Floor(mapped);
    }

    private static byte[] BuildReport(ReadOnlySpan<byte> payload, int reportLength)
    {
        if (reportLength < payload.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(reportLength), "Report length cannot be shorter than the payload.");
        }

        var report = new byte[reportLength];
        payload.CopyTo(report);
        return report;
    }
}
