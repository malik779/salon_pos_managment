using IdentityService.Application.Users;
using MediatR;

namespace IdentityService.Api;

public static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users");

        group.MapPost("", async (RegisterUserRequest request, HttpContext context, ISender sender) =>
        {
            var actor = context.User.Identity?.Name ?? "system";
            var command = new RegisterUserCommand(request.Email, request.FullName, request.BranchId, request.Role, actor);
            var result = await sender.Send(command);
            return Results.Created($"/users/{result.Id}", result);
        });

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetUserByIdQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        return app;
    }
}

public sealed record RegisterUserRequest(string Email, string FullName, Guid BranchId, string Role);
