using IdentityService.Application.Auth.Commands.GenerateToken;
using IdentityService.Application.Devices.Commands.RegisterDevice;
using IdentityService.Application.Users.Commands.CreateUser;
using IdentityService.Application.Users.Models;
using IdentityService.Application.Users.Queries.GetUser;
using IdentityService.Application.Users.Queries.ListUsers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace IdentityService.Api;

public static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/auth");
        auth.MapPost("/token", async (GenerateTokenCommand command, IMediator mediator) =>
        {
            var response = await mediator.Send(command);
            return Results.Ok(response);
        });

        var users = app.MapGroup("/users");
        users.MapPost("", async (CreateUserCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/users/{result.Id}", result);
        });

        users.MapGet("", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new ListUsersQuery());
            return Results.Ok(result);
        });

        users.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetUserQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        var devices = app.MapGroup("/devices");
        devices.MapPost("/register", async (RegisterDeviceCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        });

        return app;
    }
}
