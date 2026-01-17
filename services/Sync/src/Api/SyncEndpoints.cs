using MediatR;
using SyncService.Application.Devices;

namespace SyncService.Api;

public static class SyncEndpoints
{
    public static IEndpointRouteBuilder MapSyncEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sync");

        group.MapPost("/register", async (RegisterDeviceSyncRequest request, HttpContext context, ISender sender) =>
        {
            var actor = context.User.Identity?.Name ?? "system";
            var command = new RegisterDeviceSyncCommand(request.DeviceId, request.Platform, actor);
            var result = await sender.Send(command);
            return Results.Ok(result);
        });

        group.MapGet("/state/{deviceId:guid}", async (Guid deviceId, ISender sender) =>
        {
            var result = await sender.Send(new GetDeviceSyncStateQuery(deviceId));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        return app;
    }
}

public sealed record RegisterDeviceSyncRequest(Guid DeviceId, string Platform);
