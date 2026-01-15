using CatalogService.Application.Items;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Api;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/catalog/items");

        group.MapPost("", async (DefineCatalogItemRequest request, HttpContext context, ISender sender) =>
        {
            var actor = context.User.Identity?.Name ?? "system";
            var command = new DefineCatalogItemCommand(request.Id, request.Name, request.Type, request.Price, request.DurationMinutes, actor);
            var result = await sender.Send(command);
            return Results.Ok(result);
        });

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetCatalogItemQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapGet("/all", async (ISender sender, [FromQuery] string? searchTerm = null) =>
        {
            var query = new GetAllCatalogItemsQuery(searchTerm);
            var result = await sender.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetAllCatalogItems");

        return app;
    }
}

public sealed record DefineCatalogItemRequest(Guid? Id, string Name, string Type, decimal Price, int DurationMinutes);
