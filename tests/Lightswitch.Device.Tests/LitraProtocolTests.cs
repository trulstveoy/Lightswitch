using Lightswitch.Device;

namespace Lightswitch.Device.Tests;

public sealed class LitraProtocolTests
{
    [Fact]
    public void BuildPowerReport_On_ReturnsPaddedTwentyByteOutputReport()
    {
        var report = LitraProtocol.BuildPowerReport(isOn: true);

        Assert.Equal(20, report.Length);
        Assert.Equal([0x11, 0xFF, 0x04, 0x1C, 0x01], report.Take(5).ToArray());
        Assert.All(report.Skip(5), value => Assert.Equal(0x00, value));
    }

    [Fact]
    public void BuildPowerReport_Off_ReturnsPaddedTwentyByteOutputReport()
    {
        var report = LitraProtocol.BuildPowerReport(isOn: false);

        Assert.Equal(20, report.Length);
        Assert.Equal([0x11, 0xFF, 0x04, 0x1C, 0x00], report.Take(5).ToArray());
        Assert.All(report.Skip(5), value => Assert.Equal(0x00, value));
    }
}
