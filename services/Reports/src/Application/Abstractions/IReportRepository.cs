using ReportsService.Domain.Entities;

namespace ReportsService.Application.Abstractions;

public interface IReportRepository
{
    IEnumerable<ReportKpi> LatestKpis();
    IEnumerable<BranchDailyReport> BranchDaily(Guid branchId);
}
