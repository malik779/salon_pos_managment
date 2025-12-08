using Microsoft.EntityFrameworkCore;
using SyncOffline.Domain.Devices;

namespace SyncOffline.Application.Abstractions;

public interface ISyncDbContext
{
    DbSet<DeviceRegistration> Devices { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
