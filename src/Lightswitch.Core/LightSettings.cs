namespace Lightswitch.Core;

public sealed record LightSettings
{
    public LightState LastState { get; init; } = LightState.Default;

    public bool StartWithWindows { get; init; }
}
