using CatalogService.Application.Abstractions;
using MediatR;
using Salon.BuildingBlocks.Abstractions;

namespace CatalogService.Application.Items;

public sealed record GetCatalogItemQuery(Guid Id) : IQuery<CatalogItemDto?>;

public sealed class GetCatalogItemQueryHandler : IRequestHandler<GetCatalogItemQuery, CatalogItemDto?>
{
    private readonly ICatalogRepository _repository;

    public GetCatalogItemQueryHandler(ICatalogRepository repository)
    {
        _repository = repository;
    }

    public async Task<CatalogItemDto?> Handle(GetCatalogItemQuery request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetAsync(request.Id, cancellationToken);
        return item is null ? null : new CatalogItemDto(item.Id, item.Name, item.Type, item.Price, item.DurationMinutes);
    }
}
