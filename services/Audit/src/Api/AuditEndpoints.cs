using System.Collections.Generic;
using AuditService.Application.AuditEvents;
using MediatR;

namespace AuditService.Api;

public static class AuditEndpoints
{
    public static IEndpointRouteBuilder MapAuditEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/audit/events");

        group.MapPost("", async (StoreAuditRecordRequest request, ISender sender) =>
        {
            var command = new StoreAuditRecordCommand(request.Service, request.Action, request.Actor, request.EntityId, request.Metadata);
            var result = await sender.Send(command);
            return Results.Created($"/audit/events/{result.Id}", result);
        });

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetAuditRecordQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        return app;
    }
}

public sealed record StoreAuditRecordRequest(string Service, string Action, string Actor, string EntityId, Dictionary<string, object?> Metadata);
