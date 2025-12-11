using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<DeviceRegistrationEntity> DeviceRegistrations => Set<DeviceRegistrationEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Email).IsRequired();
            builder.Property(x => x.FullName).IsRequired();
            builder.Property(x => x.Roles).HasConversion(
                v => string.Join(';', v ?? Array.Empty<string>()),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries));
        });

        modelBuilder.Entity<DeviceRegistrationEntity>(builder =>
        {
            builder.HasKey(x => x.DeviceId);
            builder.Property(x => x.DevicePublicKey).IsRequired();
            builder.Property(x => x.Platform).IsRequired();
            builder.Property(x => x.SyncToken).IsRequired();
        });
    }
}

public class DeviceRegistrationEntity
{
    public Guid DeviceId { get; set; }
    public string DevicePublicKey { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string SyncToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}
