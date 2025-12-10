using CatalogService.Application.Abstractions;
using CatalogService.Application.Catalog.Models;
using CatalogService.Domain.Entities;
using FluentValidation;
using MediatR;

namespace CatalogService.Application.Catalog.Commands.CreateProduct;

public sealed record CreateProductCommand(string Sku, string Name, decimal Price, int InventoryQty) : IRequest<ProductDto>;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Sku).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly ICatalogRepository _repository;

    public CreateProductCommandHandler(ICatalogRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new ProductItem(Guid.NewGuid(), request.Sku, request.Name, request.Price, request.InventoryQty);
        await _repository.AddProductAsync(product, cancellationToken);
        return new ProductDto(product.Id, product.Sku, product.Name, product.Price, product.InventoryQty);
    }
}
