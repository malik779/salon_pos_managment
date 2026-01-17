using MediatR;
using Microsoft.AspNetCore.Mvc;
using StaffService.Application.StaffMembers;

namespace StaffService.Api;

public static class StaffEndpoints
{
    public static IEndpointRouteBuilder MapStaffEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/staff");

        group.MapPost("", async (RegisterStaffMemberRequest request, HttpContext context, ISender sender) =>
        {
            var actor = context.User.Identity?.Name ?? "system";
            var command = new RegisterStaffMemberCommand(request.UserId, request.DefaultBranchId, request.Role, actor);
            var result = await sender.Send(command);
            return Results.Created($"/staff/{result.Id}", result);
        });

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetStaffMemberByIdQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapGet("/all", async (ISender sender, [FromQuery] string? searchTerm = null) =>
        {
            var query = new GetAllStaffMembersQuery(searchTerm);
            var result = await sender.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetAllStaffMembers");

        return app;
    }
}

public sealed record RegisterStaffMemberRequest(Guid UserId, Guid DefaultBranchId, string Role);
