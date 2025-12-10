using CatalogService.Domain.Entities;

namespace CatalogService.Application.Abstractions;

public interface ICatalogRepository
{
    Task AddServiceAsync(ServiceItem service, CancellationToken cancellationToken);
    Task AddProductAsync(ProductItem product, CancellationToken cancellationToken);
    Task<IReadOnlyList<ServiceItem>> ListServicesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductItem>> ListProductsAsync(CancellationToken cancellationToken);
    Task<ProductItem?> GetProductAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateProductAsync(ProductItem product, CancellationToken cancellationToken);
}
