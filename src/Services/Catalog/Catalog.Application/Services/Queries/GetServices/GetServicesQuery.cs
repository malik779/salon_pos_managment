using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Models;
using BuildingBlocks.Domain.Primitives;
using Catalog.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Services.Queries.GetServices;

public sealed record ServiceSummary(Guid Id, string Name, int DurationMinutes, decimal BasePrice, string Category);

public sealed record GetServicesQuery(int PageNumber = 1, int PageSize = 50) : IQuery<PagedResult<ServiceSummary>>;

public sealed class GetServicesQueryHandler(ICatalogDbContext context)
    : IQueryHandler<GetServicesQuery, PagedResult<ServiceSummary>>
{
    public async Task<Result<PagedResult<ServiceSummary>>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
    {
        var query = context.Services
            .AsNoTracking()
            .OrderBy(x => x.Name);

        var total = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new ServiceSummary(s.Id, s.Name, s.DurationMinutes, s.BasePrice, s.Category))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<ServiceSummary>(items, request.PageNumber, request.PageSize, total);
        return Result.Success(result);
    }
}
