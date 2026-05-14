namespace Lightswitch.Core;

public static class LightLimits
{
    public const int MinBrightness = 0;
    public const int MaxBrightness = 100;
    public const int DefaultBrightness = 50;

    public const int MinTemperatureKelvin = 2700;
    public const int MaxTemperatureKelvin = 6500;
    public const int DefaultTemperatureKelvin = 4000;

    public static int ClampBrightness(int value) =>
        Math.Clamp(value, MinBrightness, MaxBrightness);

    public static int ClampTemperatureKelvin(int value) =>
        Math.Clamp(value, MinTemperatureKelvin, MaxTemperatureKelvin);
}
