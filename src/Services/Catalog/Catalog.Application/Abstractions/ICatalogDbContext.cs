using Catalog.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Abstractions;

public interface ICatalogDbContext
{
    DbSet<ServiceItem> Services { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
