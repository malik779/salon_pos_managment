using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Salon.BuildingBlocks.Data;
using SyncService.Application.Abstractions;
using SyncService.Domain;

namespace SyncService.Infrastructure.Persistence;

public sealed class SyncDbContext : ServiceDbContextBase
{
    public SyncDbContext(DbContextOptions<SyncDbContext> options) : base(options)
    {
    }

    public DbSet<DeviceSyncState> DeviceSyncStates => Set<DeviceSyncState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeviceSyncState>(builder =>
        {
            builder.ToTable("DeviceSyncStates");
            builder.HasKey(x => x.DeviceId);
            builder.Property(x => x.Platform).HasMaxLength(50).IsRequired();
        });
    }
}

internal sealed class DeviceSyncRepository : IDeviceSyncRepository
{
    private readonly SyncDbContext _context;

    public DeviceSyncRepository(SyncDbContext context)
    {
        _context = context;
    }

    public async Task<DeviceSyncState?> GetAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        return await _context.DeviceSyncStates.FindAsync(new object?[] { deviceId }, cancellationToken);
    }

    public async Task UpsertAsync(DeviceSyncState state, CancellationToken cancellationToken)
    {
        var existing = await _context.DeviceSyncStates.FindAsync(new object?[] { state.DeviceId }, cancellationToken);
        if (existing is null)
        {
            await _context.DeviceSyncStates.AddAsync(state, cancellationToken);
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(state);
        }
    }
}

public static class SyncInfrastructureRegistration
{
    public static IServiceCollection AddSyncInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<SyncDbContext>(configuration, "SyncDb");
        services.AddScoped<IDeviceSyncRepository, DeviceSyncRepository>();
        return services;
    }
}
