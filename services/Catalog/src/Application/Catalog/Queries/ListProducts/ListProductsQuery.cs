using CatalogService.Application.Abstractions;
using CatalogService.Application.Catalog.Models;
using MediatR;

namespace CatalogService.Application.Catalog.Queries.ListProducts;

public sealed record ListProductsQuery() : IRequest<IReadOnlyList<ProductDto>>;

public sealed class ListProductsQueryHandler : IRequestHandler<ListProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly ICatalogRepository _repository;

    public ListProductsQueryHandler(ICatalogRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ProductDto>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.ListProductsAsync(cancellationToken);
        return products.Select(p => new ProductDto(p.Id, p.Sku, p.Name, p.Price, p.InventoryQty)).ToList();
    }
}
