using AuditService.Application.Audit.Commands.CreateAuditEvent;
using AuditService.Application.Audit.Queries.ListAuditEvents;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AuditService.Api;

public static class AuditEndpoints
{
    public static IEndpointRouteBuilder MapAuditEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/audit/events", async (CreateAuditEventCommand command, IMediator mediator) => Results.Created("/audit/events", await mediator.Send(command)));
        app.MapGet("/audit/events", async (IMediator mediator) => Results.Ok(await mediator.Send(new ListAuditEventsQuery())));
        app.MapGet("/audit/exports/{id:guid}", (Guid id) => Results.Ok(new { exportId = id, status = "Completed" }));
        return app;
    }
}
