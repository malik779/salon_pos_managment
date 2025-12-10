using CatalogService.Application.Catalog.Commands.AdjustStock;
using CatalogService.Application.Catalog.Commands.CreateProduct;
using CatalogService.Application.Catalog.Commands.CreateService;
using CatalogService.Application.Catalog.Queries.ListProducts;
using CatalogService.Application.Catalog.Queries.ListServices;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace CatalogService.Api;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var catalog = app.MapGroup("/catalog");

        catalog.MapGet("/services", async (IMediator mediator) => Results.Ok(await mediator.Send(new ListServicesQuery())));
        catalog.MapGet("/products", async (IMediator mediator) => Results.Ok(await mediator.Send(new ListProductsQuery())));

        catalog.MapPost("/services", async (CreateServiceCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/catalog/services/{result.Id}", result);
        });

        catalog.MapPost("/products", async (CreateProductCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/catalog/products/{result.Id}", result);
        });

        catalog.MapPost("/stock/adjust", async (AdjustStockCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        });

        return app;
    }
}
