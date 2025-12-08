using BuildingBlocks.Infrastructure.Data;
using Catalog.Application.Abstractions;
using Catalog.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options)
    : AppDbContextBase(options), ICatalogDbContext
{
    public DbSet<ServiceItem> Services => Set<ServiceItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServiceItem>(builder =>
        {
            builder.ToTable("Services");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
            builder.Property(s => s.Category).IsRequired().HasMaxLength(100);
            builder.Property(s => s.BasePrice).HasPrecision(18, 2);
        });

        base.OnModelCreating(modelBuilder);
    }
}
