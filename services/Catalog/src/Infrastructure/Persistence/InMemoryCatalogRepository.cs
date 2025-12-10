using CatalogService.Application.Abstractions;
using CatalogService.Domain.Entities;

namespace CatalogService.Infrastructure.Persistence;

public sealed class InMemoryCatalogRepository : ICatalogRepository
{
    private readonly Dictionary<Guid, ServiceItem> _services = new();
    private readonly Dictionary<Guid, ProductItem> _products = new();

    public Task AddServiceAsync(ServiceItem service, CancellationToken cancellationToken)
    {
        _services[service.Id] = service;
        return Task.CompletedTask;
    }

    public Task AddProductAsync(ProductItem product, CancellationToken cancellationToken)
    {
        _products[product.Id] = product;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ServiceItem>> ListServicesAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<ServiceItem> snapshot = _services.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<IReadOnlyList<ProductItem>> ListProductsAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<ProductItem> snapshot = _products.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<ProductItem?> GetProductAsync(Guid id, CancellationToken cancellationToken)
    {
        _products.TryGetValue(id, out var product);
        return Task.FromResult(product);
    }

    public Task UpdateProductAsync(ProductItem product, CancellationToken cancellationToken)
    {
        _products[product.Id] = product;
        return Task.CompletedTask;
    }
}
