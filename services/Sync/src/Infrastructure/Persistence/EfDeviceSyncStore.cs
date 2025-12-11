using Microsoft.EntityFrameworkCore;
using SyncService.Application.Abstractions;
using SyncService.Domain.Entities;

namespace SyncService.Infrastructure.Persistence;

public sealed class EfDeviceSyncStore : IDeviceSyncStore
{
    private readonly SyncDbContext _dbContext;

    public EfDeviceSyncStore(SyncDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeviceRegistration> RegisterAsync(Guid deviceId, string platform, string publicKey, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Registrations.FirstOrDefaultAsync(x => x.DeviceId == deviceId, cancellationToken);
        if (entity is null)
        {
            entity = new DeviceRegistrationEntity { DeviceId = deviceId };
            await _dbContext.Registrations.AddAsync(entity, cancellationToken);
        }

        entity.Platform = platform;
        entity.PublicKey = publicKey;
        entity.SyncToken = Guid.NewGuid().ToString("N");
        entity.ExpiresAtUtc = DateTime.UtcNow.AddHours(8);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new DeviceRegistration(entity.DeviceId, entity.SyncToken, entity.ExpiresAtUtc);
    }

    public async Task<long> AppendOperationsAsync(Guid deviceId, IEnumerable<SyncOperation> operations, CancellationToken cancellationToken)
    {
        var currentSequence = await _dbContext.Operations.Where(x => x.DeviceId == deviceId)
            .MaxAsync(x => (long?)x.Sequence, cancellationToken) ?? 0;

        foreach (var operation in operations)
        {
            currentSequence++;
            await _dbContext.Operations.AddAsync(new SyncOperationEntity
            {
                DeviceId = deviceId,
                OperationType = operation.OperationType,
                Payload = operation.Payload,
                Sequence = currentSequence
            }, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return currentSequence;
    }

    public async Task<IEnumerable<SyncOperation>> PullAsync(Guid deviceId, long lastSequence, CancellationToken cancellationToken)
    {
        var results = await _dbContext.Operations
            .AsNoTracking()
            .Where(x => x.DeviceId == deviceId && x.Sequence > lastSequence)
            .OrderBy(x => x.Sequence)
            .ToListAsync(cancellationToken);

        return results.Select(x => new SyncOperation(x.Sequence, x.OperationType, x.Payload));
    }
}
