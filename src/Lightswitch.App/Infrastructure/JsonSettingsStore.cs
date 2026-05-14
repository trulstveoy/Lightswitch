using System.Text.Json;
using Lightswitch.Core;

namespace Lightswitch.App.Infrastructure;

public sealed class JsonSettingsStore : ISettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private readonly string _settingsPath;

    public JsonSettingsStore()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var settingsDirectory = Path.Combine(appData, "Lightswitch");
        _settingsPath = Path.Combine(settingsDirectory, "settings.json");
    }

    public async Task<LightSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_settingsPath))
        {
            return new LightSettings();
        }

        await using var stream = File.OpenRead(_settingsPath);
        var settings = await JsonSerializer.DeserializeAsync<LightSettings>(stream, JsonOptions, cancellationToken);

        return settings is null
            ? new LightSettings()
            : settings with { LastState = settings.LastState.Normalize() };
    }

    public async Task SaveAsync(LightSettings settings, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath)!);

        var normalized = settings with { LastState = settings.LastState.Normalize() };
        await using var stream = File.Create(_settingsPath);
        await JsonSerializer.SerializeAsync(stream, normalized, JsonOptions, cancellationToken);
    }
}
