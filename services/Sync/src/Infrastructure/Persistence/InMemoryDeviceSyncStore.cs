using SyncService.Application.Abstractions;
using SyncService.Domain.Entities;

namespace SyncService.Infrastructure.Persistence;

public sealed class InMemoryDeviceSyncStore : IDeviceSyncStore
{
    private readonly Dictionary<Guid, DeviceRegistration> _registrations = new();
    private readonly Dictionary<Guid, List<SyncOperation>> _operations = new();

    public Task<DeviceRegistration> RegisterAsync(Guid deviceId, string platform, string publicKey, CancellationToken cancellationToken)
    {
        var registration = new DeviceRegistration(deviceId, Guid.NewGuid().ToString("N"), DateTime.UtcNow.AddHours(8));
        _registrations[deviceId] = registration;
        return Task.FromResult(registration);
    }

    public Task<long> AppendOperationsAsync(Guid deviceId, IEnumerable<SyncOperation> operations, CancellationToken cancellationToken)
    {
        if (!_operations.TryGetValue(deviceId, out var list))
        {
            list = new List<SyncOperation>();
            _operations[deviceId] = list;
        }

        foreach (var operation in operations)
        {
            list.Add(operation with { Sequence = list.Count + 1 });
        }

        return Task.FromResult((long)list.Count);
    }

    public Task<IEnumerable<SyncOperation>> PullAsync(Guid deviceId, long lastSequence, CancellationToken cancellationToken)
    {
        if (!_operations.TryGetValue(deviceId, out var list))
        {
            return Task.FromResult<IEnumerable<SyncOperation>>(Array.Empty<SyncOperation>());
        }

        var result = list.Where(op => op.Sequence > lastSequence).OrderBy(op => op.Sequence).ToList();
        return Task.FromResult<IEnumerable<SyncOperation>>(result);
    }
}
