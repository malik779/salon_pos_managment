using SyncService.Domain;

namespace SyncService.Application.Abstractions;

public interface IDeviceSyncRepository
{
    Task<DeviceSyncState?> GetAsync(Guid deviceId, CancellationToken cancellationToken);
    Task UpsertAsync(DeviceSyncState state, CancellationToken cancellationToken);
}
