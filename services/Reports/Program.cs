using MediatR;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("reports-service");

builder.Services.AddSingleton<ReportStore>();
builder.Services.AddMediatR(typeof(Program));

var app = builder.Build();
app.UseServiceDefaults();

app.MapGet("/reports/kpis", (ReportStore store) => Results.Ok(store.LatestKpis()));
app.MapGet("/reports/branches/{branchId:guid}/daily", (Guid branchId, ReportStore store) => Results.Ok(store.BranchDaily(branchId)));
app.MapPost("/reports/rebuild", async (RebuildReportsCommand command, IMediator mediator) => Results.Accepted(value: await mediator.Send(command)));

app.Run();

record ReportKpi(string Name, decimal Value);
record BranchDailyReport(DateOnly BusinessDate, decimal Sales, int Bookings, decimal CommissionPayout);
record RebuildReportsCommand(Guid BranchId) : IRequest<string>;

class ReportStore
{
    public IEnumerable<ReportKpi> LatestKpis() =>
        new[]
        {
            new ReportKpi("total_sales", 10000m),
            new ReportKpi("active_clients", 250),
            new ReportKpi("avg_ticket", 80m)
        };

    public IEnumerable<BranchDailyReport> BranchDaily(Guid branchId) =>
        Enumerable.Range(0, 7)
            .Select(offset => new BranchDailyReport(DateOnly.FromDateTime(DateTime.Today.AddDays(-offset)), 1000 + offset * 25, 30 + offset, 200 + offset * 10));
}

class RebuildReportsHandler : IRequestHandler<RebuildReportsCommand, string>
{
    public Task<string> Handle(RebuildReportsCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"rebuild_started_for_{request.BranchId}");
    }
}
