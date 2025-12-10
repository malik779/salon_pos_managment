using BranchService.Application.Branches.Commands.CreateBranch;
using BranchService.Application.Branches.Queries.GetBranch;
using BranchService.Application.Branches.Queries.ListBranches;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BranchService.Api;

public static class BranchEndpoints
{
    public static IEndpointRouteBuilder MapBranchEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/branches");

        group.MapGet("", async (IMediator mediator) => Results.Ok(await mediator.Send(new ListBranchesQuery())));

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetBranchQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("", async (CreateBranchCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/branches/{result.Id}", result);
        });

        return app;
    }
}
