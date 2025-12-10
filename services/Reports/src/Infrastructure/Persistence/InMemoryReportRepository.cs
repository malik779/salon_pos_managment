using ReportsService.Application.Abstractions;
using ReportsService.Domain.Entities;

namespace ReportsService.Infrastructure.Persistence;

public sealed class InMemoryReportRepository : IReportRepository
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
