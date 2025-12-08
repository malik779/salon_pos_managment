using BuildingBlocks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SyncOffline.Application.Abstractions;
using SyncOffline.Domain.Devices;

namespace SyncOffline.Infrastructure.Persistence;

public sealed class SyncDbContext(DbContextOptions<SyncDbContext> options)
    : AppDbContextBase(options), ISyncDbContext
{
    public DbSet<DeviceRegistration> Devices => Set<DeviceRegistration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeviceRegistration>(builder =>
        {
            builder.ToTable("Devices");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.DeviceId).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Platform).IsRequired().HasMaxLength(50);
            builder.Property(x => x.SyncToken).IsRequired().HasMaxLength(64);
        });

        base.OnModelCreating(modelBuilder);
    }
}
