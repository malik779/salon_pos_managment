using CatalogService.Application.Abstractions;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Persistence;

public sealed class EfCatalogRepository : ICatalogRepository
{
    private readonly CatalogDbContext _dbContext;

    public EfCatalogRepository(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddServiceAsync(ServiceItem service, CancellationToken cancellationToken)
    {
        await _dbContext.Services.AddAsync(service, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddProductAsync(ProductItem product, CancellationToken cancellationToken)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ServiceItem>> ListServicesAsync(CancellationToken cancellationToken)
        => await _dbContext.Services.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ProductItem>> ListProductsAsync(CancellationToken cancellationToken)
        => await _dbContext.Products.AsNoTracking().ToListAsync(cancellationToken);

    public Task<ProductItem?> GetProductAsync(Guid id, CancellationToken cancellationToken)
        => _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task UpdateProductAsync(ProductItem product, CancellationToken cancellationToken)
    {
        _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
