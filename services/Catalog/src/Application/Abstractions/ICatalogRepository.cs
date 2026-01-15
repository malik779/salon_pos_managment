using CatalogService.Domain;

namespace CatalogService.Application.Abstractions;

public interface ICatalogRepository
{
    Task<CatalogItem?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task UpsertAsync(CatalogItem item, CancellationToken cancellationToken);
    /// <summary>
    /// Gets all catalog items without pagination
    /// Optimized for read-only queries using AsNoTracking
    /// </summary>
    Task<List<CatalogItem>> GetAllAsync(string? searchTerm, CancellationToken cancellationToken);
}
