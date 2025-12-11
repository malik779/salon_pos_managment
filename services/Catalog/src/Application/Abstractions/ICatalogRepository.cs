using CatalogService.Domain;

namespace CatalogService.Application.Abstractions;

public interface ICatalogRepository
{
    Task<CatalogItem?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task UpsertAsync(CatalogItem item, CancellationToken cancellationToken);
}
