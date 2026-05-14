namespace Lightswitch.Core;

public interface ILitraService
{
    event EventHandler<DeviceStatus>? StatusChanged;

    DeviceStatus Status { get; }

    LightState DesiredState { get; }

    Task<DeviceStatus> RefreshAsync(CancellationToken cancellationToken = default);

    Task ApplyAsync(LightState state, CancellationToken cancellationToken = default);
}
