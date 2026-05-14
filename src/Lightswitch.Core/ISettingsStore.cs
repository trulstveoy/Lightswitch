namespace Lightswitch.Core;

public interface ISettingsStore
{
    Task<LightSettings> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(LightSettings settings, CancellationToken cancellationToken = default);
}
