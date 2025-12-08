using BuildingBlocks.Infrastructure.Data;
using ClientManagement.Application.Abstractions;
using ClientManagement.Domain.Clients;
using Microsoft.EntityFrameworkCore;

namespace ClientManagement.Infrastructure.Persistence;

public sealed class ClientDbContext(DbContextOptions<ClientDbContext> options)
    : AppDbContextBase(options), IClientDbContext
{
    public DbSet<ClientProfile> Clients => Set<ClientProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClientProfile>(builder =>
        {
            builder.ToTable("Clients");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
            builder.Property(x => x.Phone).IsRequired().HasMaxLength(50);
        });

        base.OnModelCreating(modelBuilder);
    }
}
