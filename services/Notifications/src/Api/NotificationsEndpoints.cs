using System.Collections.Generic;
using MediatR;
using NotificationsService.Application.Notifications;

namespace NotificationsService.Api;

public static class NotificationsEndpoints
{
    public static IEndpointRouteBuilder MapNotificationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/notifications");

        group.MapPost("/send", async (SendNotificationRequest request, HttpContext context, ISender sender) =>
        {
            var actor = context.User.Identity?.Name ?? "system";
            var command = new SendNotificationCommand(request.ClientId, request.Channel, request.TemplateCode, request.Tokens, actor);
            var result = await sender.Send(command);
            return Results.Ok(result);
        });

        return app;
    }
}

public sealed record SendNotificationRequest(Guid ClientId, string Channel, string TemplateCode, Dictionary<string, string> Tokens);
