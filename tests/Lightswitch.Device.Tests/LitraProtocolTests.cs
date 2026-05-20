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

    [Theory]
    [InlineData(0, 20)]
    [InlineData(50, 135)]
    [InlineData(100, 250)]
    public void BuildBrightnessReport_MapsPercentToDeviceBrightness(int brightness, int expectedDeviceBrightness)
    {
        var report = LitraProtocol.BuildBrightnessReport(brightness);

        Assert.Equal(20, report.Length);
        Assert.Equal([0x11, 0xFF, 0x04, 0x4C, 0x00, (byte)expectedDeviceBrightness], report.Take(6).ToArray());
        Assert.All(report.Skip(6), value => Assert.Equal(0x00, value));
    }

    [Fact]
    public void BuildTemperatureReport_EncodesKelvinAsHighThenLowByte()
    {
        var report = LitraProtocol.BuildTemperatureReport(6500);

        Assert.Equal(20, report.Length);
        Assert.Equal([0x11, 0xFF, 0x04, 0x9C, 0x19, 0x64], report.Take(6).ToArray());
        Assert.All(report.Skip(6), value => Assert.Equal(0x00, value));
    }
}
