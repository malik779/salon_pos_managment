using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportsService.Application.Reports;

namespace ReportsService.Api;

public static class ReportsEndpoints
{
    public static IEndpointRouteBuilder MapReportsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/reports");

        group.MapPost("/kpis", async (UpsertDailyTotalsRequest request, HttpContext context, ISender sender) =>
        {
            var actor = context.User.Identity?.Name ?? "system";
            var command = new UpsertDailyTotalsCommand(request.BranchId, request.Day, request.TotalRevenue, actor);
            var result = await sender.Send(command);
            return Results.Ok(result);
        });

        group.MapGet("/kpis", async ([AsParameters] GetDailyTotalsParameters parameters, ISender sender) =>
        {
            var result = await sender.Send(new GetDailyTotalsQuery(parameters.BranchId, parameters.Day));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        return app;
    }
}

public sealed record UpsertDailyTotalsRequest(Guid BranchId, DateOnly Day, decimal TotalRevenue);
public sealed record GetDailyTotalsParameters(Guid BranchId, DateOnly Day);
