using CatalogService.Application.Abstractions;
using CatalogService.Application.Catalog.Models;
using FluentValidation;
using MediatR;

namespace CatalogService.Application.Catalog.Commands.AdjustStock;

public sealed record AdjustStockCommand(Guid ProductId, int DeltaQuantity) : IRequest<ProductDto>;

public sealed class AdjustStockCommandValidator : AbstractValidator<AdjustStockCommand>
{
    public AdjustStockCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}

public sealed class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, ProductDto>
{
    private readonly ICatalogRepository _repository;

    public AdjustStockCommandHandler(ICatalogRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetProductAsync(request.ProductId, cancellationToken)
            ?? throw new KeyNotFoundException("product_not_found");

        product.AdjustInventory(request.DeltaQuantity);
        await _repository.UpdateProductAsync(product, cancellationToken);
        return new ProductDto(product.Id, product.Sku, product.Name, product.Price, product.InventoryQty);
    }
}
