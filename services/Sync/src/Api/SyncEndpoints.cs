using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SyncService.Application.Sync.Commands.PushSyncBatch;
using SyncService.Application.Sync.Commands.RegisterDevice;
using SyncService.Application.Sync.Queries.PullChanges;

namespace SyncService.Api;

public static class SyncEndpoints
{
    public static IEndpointRouteBuilder MapSyncEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/sync/register", async (RegisterDeviceCommand command, IMediator mediator) => Results.Ok(await mediator.Send(command)));
        app.MapPost("/sync/push", async (PushSyncBatchCommand command, IMediator mediator) => Results.Ok(await mediator.Send(command)));
        app.MapGet("/sync/pull", async (Guid deviceId, long lastSequence, IMediator mediator) => Results.Ok(await mediator.Send(new PullChangesQuery(deviceId, lastSequence))));
        return app;
    }
}
