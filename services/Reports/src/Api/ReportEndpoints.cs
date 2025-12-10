using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ReportsService.Application.Reports.Commands.RebuildReports;
using ReportsService.Application.Reports.Queries.GetBranchDaily;
using ReportsService.Application.Reports.Queries.GetKpis;

namespace ReportsService.Api;

public static class ReportEndpoints
{
    public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/reports/kpis", async (IMediator mediator) => Results.Ok(await mediator.Send(new GetKpisQuery())));
        app.MapGet("/reports/branches/{branchId:guid}/daily", async (Guid branchId, IMediator mediator) => Results.Ok(await mediator.Send(new GetBranchDailyQuery(branchId))));
        app.MapPost("/reports/rebuild", async (RebuildReportsCommand command, IMediator mediator) => Results.Accepted(value: await mediator.Send(command)));
        return app;
    }
}
