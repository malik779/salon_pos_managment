using Microsoft.EntityFrameworkCore;
using SyncService.Domain.Entities;

namespace SyncService.Infrastructure.Persistence;

public class SyncDbContext : DbContext
{
    public SyncDbContext(DbContextOptions<SyncDbContext> options) : base(options)
    {
    }

    public DbSet<DeviceRegistrationEntity> Registrations => Set<DeviceRegistrationEntity>();
    public DbSet<SyncOperationEntity> Operations => Set<SyncOperationEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeviceRegistrationEntity>(builder =>
        {
            builder.HasKey(x => x.DeviceId);
            builder.Property(x => x.SyncToken).IsRequired();
        });

        modelBuilder.Entity<SyncOperationEntity>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.DeviceId, x.Sequence });
        });
    }
}

public class DeviceRegistrationEntity
{
    public Guid DeviceId { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string SyncToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}

public class SyncOperationEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeviceId { get; set; }
    public long Sequence { get; set; }
    public string OperationType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
}
