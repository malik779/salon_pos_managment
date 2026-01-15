using IdentityService.Application.Authentication;
using IdentityService.Application.Users;
using MediatR;

namespace IdentityService.Api;

public static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        // Authentication endpoints (public)
        var authGroup = app.MapGroup("/api/auth");
        
        authGroup.MapPost("/token", async (LoginRequest request, ISender sender) =>
        {
             var command = new LoginCommand(request.Email, request.Password);
            var result = await sender.Send(command);
            return Results.Ok(result);
        });

        authGroup.MapPost("/register", async (RegisterRequest request, ISender sender) =>
        {
            var command = new RegisterCommand(request.Email, request.Password, request.FullName, request.BranchId, request.Role);
            var result = await sender.Send(command);
            return Results.Created($"/users/{result.UserId}", result);
        });

        authGroup.MapPost("/forgot-password", async (ForgotPasswordRequest request, ISender sender) =>
        {
            var command = new ForgotPasswordCommand(request.Email);
            var result = await sender.Send(command);
            return Results.Ok(result);
        });

        authGroup.MapPost("/reset-password", async (ResetPasswordRequest request, ISender sender) =>
        {
            var command = new ResetPasswordCommand(request.Email, request.ResetToken, request.NewPassword);
            var result = await sender.Send(command);
            return Results.Ok(result);
        });

        authGroup.MapPost("/refresh", async (RefreshTokenRequest request, ISender sender) =>
        {
            var command = new RefreshTokenCommand(request.RefreshToken);
            var result = await sender.Send(command);
            return Results.Ok(result);
        });

        // User management endpoints (protected)
        var usersGroup = app.MapGroup("/users");

        usersGroup.MapPost("", async (RegisterUserRequest request, HttpContext context, ISender sender) =>
        {
            var actor = context.User.Identity?.Name ?? "system";
            var command = new RegisterUserCommand(request.Email, request.FullName, request.BranchId, request.Role, actor);
            var result = await sender.Send(command);
            return Results.Created($"/users/{result.Id}", result);
        });

        usersGroup.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetUserByIdQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        return app;
    }
}

public sealed record LoginRequest(string Email, string Password);
public sealed record RegisterRequest(string Email, string Password, string FullName, Guid BranchId, string Role);
public sealed record ForgotPasswordRequest(string Email);
public sealed record ResetPasswordRequest(string Email, string ResetToken, string NewPassword);
public sealed record RefreshTokenRequest(string RefreshToken);
public sealed record RegisterUserRequest(string Email, string FullName, Guid BranchId, string Role);
