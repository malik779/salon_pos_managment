using ClientService.Application.Clients;
using MediatR;

namespace ClientService.Api;

public static class ClientEndpoints
{
    public static IEndpointRouteBuilder MapClientEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/clients");

        group.MapPost("", async (CreateClientProfileRequest request, HttpContext context, ISender sender) =>
        {
            var actor = context.User.Identity?.Name ?? "system";
            var command = new CreateClientProfileCommand(request.FirstName, request.LastName, request.Phone, request.Email, actor);
            var result = await sender.Send(command);
            return Results.Created($"/clients/{result.Id}", result);
        });

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetClientProfileByIdQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        return app;
    }
}

public sealed record CreateClientProfileRequest(string FirstName, string LastName, string Phone, string Email);
