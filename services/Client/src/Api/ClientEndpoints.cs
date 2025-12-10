using ClientService.Application.Clients.Commands.CaptureConsent;
using ClientService.Application.Clients.Commands.CreateClient;
using ClientService.Application.Clients.Queries.ListClients;
using ClientService.Application.Clients.Queries.ListConsents;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ClientService.Api;

public static class ClientEndpoints
{
    public static IEndpointRouteBuilder MapClientEndpoints(this IEndpointRouteBuilder app)
    {
        var clients = app.MapGroup("/clients");

        clients.MapGet("", async (IMediator mediator) => Results.Ok(await mediator.Send(new ListClientsQuery())));

        clients.MapPost("", async (CreateClientCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/clients/{result.Id}", result);
        });

        clients.MapGet("/{id:guid}/consents", async (Guid id, IMediator mediator) => Results.Ok(await mediator.Send(new ListConsentsQuery(id))));

        clients.MapPost("/{id:guid}/consents", async (Guid id, CaptureConsentCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command with { ClientId = id });
            return Results.Ok(result);
        });

        return app;
    }
}
