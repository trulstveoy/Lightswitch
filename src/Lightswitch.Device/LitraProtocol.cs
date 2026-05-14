namespace Lightswitch.Device;

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
