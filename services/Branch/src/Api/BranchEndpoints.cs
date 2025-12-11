using BranchService.Application.Branches;
using MediatR;

namespace BranchService.Api;

public static class BranchEndpoints
{
    public static IEndpointRouteBuilder MapBranchEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/branches");

        group.MapPost("", async (CreateBranchRequest request, HttpContext context, ISender sender) =>
        {
            var actor = context.User.Identity?.Name ?? "system";
            var command = new CreateBranchCommand(request.Name, request.Timezone, request.Address, actor);
            var result = await sender.Send(command);
            return Results.Created($"/branches/{result.Id}", result);
        });

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetBranchByIdQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        return app;
    }
}

public sealed record CreateBranchRequest(string Name, string Timezone, string Address);
