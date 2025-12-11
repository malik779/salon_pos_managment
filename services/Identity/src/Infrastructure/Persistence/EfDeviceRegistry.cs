using IdentityService.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence;

public sealed class EfDeviceRegistry : IDeviceRegistry
{
    private readonly IdentityDbContext _dbContext;

    public EfDeviceRegistry(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task RegisterAsync(DeviceRegistration registration, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.DeviceRegistrations.FirstOrDefaultAsync(x => x.DeviceId == registration.DeviceId, cancellationToken);
        if (entity is null)
        {
            entity = new DeviceRegistrationEntity
            {
                DeviceId = registration.DeviceId
            };
            await _dbContext.DeviceRegistrations.AddAsync(entity, cancellationToken);
        }

        entity.DevicePublicKey = registration.DevicePublicKey;
        entity.Platform = registration.Platform;
        entity.SyncToken = registration.SyncToken;
        entity.ExpiresAtUtc = registration.ExpiresAtUtc;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
