using CatalogService.Application.Abstractions;
using CatalogService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Salon.BuildingBlocks.Data;

namespace CatalogService.Infrastructure.Persistence;

public sealed class CatalogDbContext : ServiceDbContextBase
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatalogItem>(builder =>
        {
            builder.ToTable("CatalogItems");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Type).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        });
    }
}

internal sealed class CatalogRepository : ICatalogRepository
{
    private readonly CatalogDbContext _context;

    public CatalogRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<CatalogItem?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.CatalogItems.FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task UpsertAsync(CatalogItem item, CancellationToken cancellationToken)
    {
        var existing = await _context.CatalogItems.FindAsync(new object?[] { item.Id }, cancellationToken);
        if (existing is null)
        {
            await _context.CatalogItems.AddAsync(item, cancellationToken);
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(item);
        }
    }
}

public static class CatalogInfrastructureRegistration
{
    public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<CatalogDbContext>(configuration, "CatalogDb");
        services.AddScoped<ICatalogRepository, CatalogRepository>();
        return services;
    }
}
