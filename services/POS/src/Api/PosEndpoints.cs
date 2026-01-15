using MediatR;
using Microsoft.AspNetCore.Mvc;
using PosService.Application.Invoices;

namespace PosService.Api;

public static class PosEndpoints
{
    public static IEndpointRouteBuilder MapPosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/pos/invoices");

        group.MapPost("", async (FinalizeInvoiceRequest request, HttpContext context, ISender sender) =>
        {
            var actor = context.User.Identity?.Name ?? "system";
            var command = new FinalizeInvoiceCommand(request.BranchId, request.ClientId, request.Total, actor);
            var result = await sender.Send(command);
            return Results.Created($"/pos/invoices/{result.Id}", result);
        });

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetInvoiceByIdQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapGet("/all", async (ISender sender, [FromQuery] string? searchTerm = null) =>
        {
            var query = new GetAllInvoicesQuery(searchTerm);
            var result = await sender.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetAllInvoices");

        return app;
    }
}

public sealed record FinalizeInvoiceRequest(Guid BranchId, Guid ClientId, decimal Total);
