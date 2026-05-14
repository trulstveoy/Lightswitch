using Lightswitch.Core;

namespace Lightswitch.Core.Tests;

public sealed class LightStateTests
{
    [Fact]
    public void Normalize_ValuesOutsideLimits_ClampsToSupportedRanges()
    {
        var state = new LightState
        {
            IsOn = true,
            Brightness = 250,
            TemperatureKelvin = 1000
        };

        var normalized = state.Normalize();

        Assert.True(normalized.IsOn);
        Assert.Equal(LightLimits.MaxBrightness, normalized.Brightness);
        Assert.Equal(LightLimits.MinTemperatureKelvin, normalized.TemperatureKelvin);
    }

    [Fact]
    public void Default_UsesExpectedStartupValues()
    {
        var state = LightState.Default;

        Assert.True(state.IsOn);
        Assert.Equal(LightLimits.DefaultBrightness, state.Brightness);
        Assert.Equal(LightLimits.DefaultTemperatureKelvin, state.TemperatureKelvin);
    }
}
