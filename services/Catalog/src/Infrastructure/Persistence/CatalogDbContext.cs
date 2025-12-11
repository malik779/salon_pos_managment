using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Persistence;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<ServiceItem> Services => Set<ServiceItem>();
    public DbSet<ProductItem> Products => Set<ProductItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServiceItem>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
        });

        modelBuilder.Entity<ProductItem>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Sku).IsRequired();
            builder.Property(x => x.Name).IsRequired();
        });
    }
}
