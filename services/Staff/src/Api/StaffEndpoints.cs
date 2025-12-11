using MediatR;
using StaffService.Application.StaffMembers;

namespace StaffService.Api;

public static class StaffEndpoints
{
    public static IEndpointRouteBuilder MapStaffEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/staff");

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

        return app;
    }
}

public sealed record RegisterStaffMemberRequest(Guid UserId, Guid DefaultBranchId, string Role);
