using BranchService.Application.Branches;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BranchResponse = BranchService.Application.Branches.BranchDto;
using PaginatedBranchResponse = BranchService.Application.Branches.PaginatedResponse<BranchService.Application.Branches.BranchDto>;

namespace BranchService.Api;

public static class BranchEndpoints
{
    public static IEndpointRouteBuilder MapBranchEndpoints(this IEndpointRouteBuilder app)
    {
        // Use tag for OpenAPI/Swagger documentation
        var group = app.MapGroup("/api/branches")
                       .WithTags("Branches");
                       //.WithOpenApi(); // Enable OpenAPI support

        // POST /api/branches
        group.MapPost("Create", async (CreateBranchRequest request, HttpContext context, ISender sender) =>
        {
            var actor = context.User.Identity?.Name ?? "system";
            var command = new CreateBranchCommand(request.Name, request.Timezone, request.Address, actor);
            var result = await sender.Send(command);

            // Use nameof or constants for route names
            return Results.CreatedAtRoute(
                routeName: "GetBranchById",
                routeValues: new { id = result.Id },
                value: result);
        })
        .Produces<BranchResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem() // For automatic validation
        .WithName("CreateBranch");

        // GET /api/branches/{id}
        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetBranchByIdQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .Produces<BranchResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithName("GetBranchById"); // Named route for CreatedAtRoute

        // GET /api/branches
        group.MapGet("", async (ISender sender,
                                [AsParameters] GetBranchesQueryParams queryParams) =>
        {
            var query = new GetBranchesQuery(
                queryParams.PageNumber,
                queryParams.PageSize,
                queryParams.SearchTerm);
            var result = await sender.Send(query);
            return Results.Ok(result);
        })
        .Produces<PaginatedBranchResponse>()
        .WithName("GetBranches");

        // GET /api/branches/all
        group.MapGet("/all", async (ISender sender,
                                    [FromQuery] string? searchTerm = null) =>
        {
            var query = new GetAllBranchesQuery(searchTerm);
            var result = await sender.Send(query);
            return Results.Ok(result);
        })
        .Produces<List<BranchResponse>>()
        .WithName("GetAllBranches");

        // PUT /api/branches/{id}
        group.MapPut("/{id:guid}", async (Guid id,
                                          UpdateBranchRequest request,
                                          HttpContext context,
                                          ISender sender) =>
        {
            var actor = context.User.Identity?.Name ?? "system";
            var command = new UpdateBranchCommand(id, request.Name, request.Timezone, request.Address, actor);
            await sender.Send(command);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem();

        // DELETE /api/branches/{id}
        group.MapDelete("/{id:guid}", async (Guid id, HttpContext context, ISender sender) =>
        {
            var actor = context.User.Identity?.Name ?? "system";
            var command = new DeleteBranchCommand(id, actor);
            await sender.Send(command);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}

public sealed record CreateBranchRequest(string Name, string Timezone, string Address);

public sealed record UpdateBranchRequest(string Name, string Timezone, string Address);

// Supporting classes
public record GetBranchesQueryParams(
    [FromQuery] int PageNumber = 1,
    [FromQuery] int PageSize = 20,
    [FromQuery] string? SearchTerm = null);