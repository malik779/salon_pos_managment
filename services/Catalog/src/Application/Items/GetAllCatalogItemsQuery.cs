using CatalogService.Application.Abstractions;
using MediatR;
using Salon.BuildingBlocks.Abstractions;

namespace CatalogService.Application.Items;

/// <summary>
/// Query to fetch all catalog items without pagination
/// Optimized for dropdowns, selects, and other UI components that need complete lists
/// Performance: Uses AsNoTracking for read-only queries to reduce memory overhead
/// </summary>
public sealed record GetAllCatalogItemsQuery(string? SearchTerm = null) : IQuery<List<CatalogItemDto>>;

public sealed class GetAllCatalogItemsQueryHandler : IRequestHandler<GetAllCatalogItemsQuery, List<CatalogItemDto>>
{
    private readonly ICatalogRepository _repository;

    public GetAllCatalogItemsQueryHandler(ICatalogRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CatalogItemDto>> Handle(GetAllCatalogItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(request.SearchTerm, cancellationToken);
        return items.Select(i => new CatalogItemDto(i.Id, i.Name, i.Type, i.Price, i.DurationMinutes)).ToList();
    }
}
