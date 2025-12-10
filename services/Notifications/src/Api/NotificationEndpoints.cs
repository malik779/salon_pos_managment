using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NotificationsService.Application.Notifications.Commands.CreateTemplate;
using NotificationsService.Application.Notifications.Commands.SendNotification;
using NotificationsService.Application.Notifications.Queries.ListTemplates;

namespace NotificationsService.Api;

public static class NotificationEndpoints
{
    public static IEndpointRouteBuilder MapNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/notifications/send", async (SendNotificationCommand command, IMediator mediator) => Results.Ok(await mediator.Send(command)));
        app.MapPost("/templates", async (CreateTemplateCommand command, IMediator mediator) => Results.Created("/templates", await mediator.Send(command)));
        app.MapGet("/templates", async (IMediator mediator) => Results.Ok(await mediator.Send(new ListTemplatesQuery())));
        return app;
    }
}
