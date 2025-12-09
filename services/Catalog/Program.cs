using FluentValidation;
using MediatR;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("catalog-service");

builder.Services.AddSingleton<CatalogStore>();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();
app.UseServiceDefaults();

var catalog = app.MapGroup("/catalog");

catalog.MapGet("/services", (CatalogStore store) => Results.Ok(store.Services));
catalog.MapGet("/products", (CatalogStore store) => Results.Ok(store.Products));
catalog.MapPost("/services", async (CreateServiceCommand command, IMediator mediator) => Results.Created("/catalog/services", await mediator.Send(command)));
catalog.MapPost("/products", async (CreateProductCommand command, IMediator mediator) => Results.Created("/catalog/products", await mediator.Send(command)));

catalog.MapPost("/stock/adjust", async (AdjustStockCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command);
    return Results.Ok(result);
});

app.Run();

record ServiceDto(Guid Id, string Name, int DurationMinutes, decimal BasePrice);
record ProductDto(Guid Id, string Sku, string Name, decimal Price, int InventoryQty);
record StockAdjustmentDto(Guid ProductId, int NewQuantity);

record CreateServiceCommand(string Name, int DurationMinutes, decimal BasePrice) : IRequest<ServiceDto>;
record CreateProductCommand(string Sku, string Name, decimal Price, int InventoryQty) : IRequest<ProductDto>;
record AdjustStockCommand(Guid ProductId, int DeltaQuantity) : IRequest<StockAdjustmentDto>;

class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.DurationMinutes).GreaterThan(0);
        RuleFor(x => x.BasePrice).GreaterThan(0);
    }
}

class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Sku).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

class AdjustStockCommandValidator : AbstractValidator<AdjustStockCommand>
{
    public AdjustStockCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}

class CatalogStore
{
    public List<ServiceDto> Services { get; } = new();
    public List<ProductDto> Products { get; } = new();

    public ProductDto? FindProduct(Guid id) => Products.FirstOrDefault(x => x.Id == id);
}

class CreateServiceHandler : IRequestHandler<CreateServiceCommand, ServiceDto>
{
    private readonly CatalogStore _store;

    public CreateServiceHandler(CatalogStore store)
    {
        _store = store;
    }

    public Task<ServiceDto> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = new ServiceDto(Guid.NewGuid(), request.Name, request.DurationMinutes, request.BasePrice);
        _store.Services.Add(service);
        return Task.FromResult(service);
    }
}

class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly CatalogStore _store;

    public CreateProductHandler(CatalogStore store)
    {
        _store = store;
    }

    public Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new ProductDto(Guid.NewGuid(), request.Sku, request.Name, request.Price, request.InventoryQty);
        _store.Products.Add(product);
        return Task.FromResult(product);
    }
}

class AdjustStockHandler : IRequestHandler<AdjustStockCommand, StockAdjustmentDto>
{
    private readonly CatalogStore _store;

    public AdjustStockHandler(CatalogStore store)
    {
        _store = store;
    }

    public Task<StockAdjustmentDto> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var product = _store.FindProduct(request.ProductId) ?? throw new KeyNotFoundException("product_not_found");
        var newQty = product.InventoryQty + request.DeltaQuantity;
        var updated = product with { InventoryQty = newQty };
        _store.Products.Remove(product);
        _store.Products.Add(updated);
        return Task.FromResult(new StockAdjustmentDto(updated.Id, updated.InventoryQty));
    }
}
