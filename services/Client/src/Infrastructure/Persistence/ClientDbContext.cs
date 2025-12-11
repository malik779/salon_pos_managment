using ClientService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClientService.Infrastructure.Persistence;

public class ClientDbContext : DbContext
{
    public ClientDbContext(DbContextOptions<ClientDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ClientConsentEntity> ClientConsents => Set<ClientConsentEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FirstName).IsRequired();
            builder.Property(x => x.LastName).IsRequired();
            builder.Property(x => x.Phone).IsRequired();
        });

        modelBuilder.Entity<ClientConsentEntity>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Version).IsRequired();
            builder.HasIndex(x => new { x.ClientId, x.Version });
        });
    }
}

public class ClientConsentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClientId { get; set; }
    public string Version { get; set; } = string.Empty;
    public DateTime CapturedAtUtc { get; set; }
}
