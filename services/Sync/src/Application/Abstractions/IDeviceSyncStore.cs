using SyncService.Domain.Entities;

namespace SyncService.Application.Abstractions;

public interface IDeviceSyncStore
{
    Task<DeviceRegistration> RegisterAsync(Guid deviceId, string platform, string publicKey, CancellationToken cancellationToken);
    Task<long> AppendOperationsAsync(Guid deviceId, IEnumerable<SyncOperation> operations, CancellationToken cancellationToken);
    Task<IEnumerable<SyncOperation>> PullAsync(Guid deviceId, long lastSequence, CancellationToken cancellationToken);
}
