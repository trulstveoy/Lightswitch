namespace Lightswitch.Core;

public sealed record LightState
{
    public static LightState Default { get; } = new();

    public bool IsOn { get; init; } = true;

    public int Brightness { get; init; } = LightLimits.DefaultBrightness;

    public int TemperatureKelvin { get; init; } = LightLimits.DefaultTemperatureKelvin;

    public LightState Normalize() =>
        this with
        {
            Brightness = LightLimits.ClampBrightness(Brightness),
            TemperatureKelvin = LightLimits.ClampTemperatureKelvin(TemperatureKelvin)
        };
}
