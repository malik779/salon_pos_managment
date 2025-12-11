using Microsoft.EntityFrameworkCore;
using ReportsService.Application.Abstractions;
using ReportsService.Domain.Entities;

namespace ReportsService.Infrastructure.Persistence;

public sealed class EfReportRepository : IReportRepository
{
    private readonly ReportsDbContext _dbContext;

    public EfReportRepository(ReportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<ReportKpi> LatestKpis()
    {
        if (!_dbContext.Kpis.Any())
        {
            _dbContext.Kpis.AddRange(new ReportKpi("total_sales", 10000m), new ReportKpi("active_clients", 250), new ReportKpi("avg_ticket", 80m));
            _dbContext.SaveChanges();
        }
        return _dbContext.Kpis.AsNoTracking().ToList();
    }

    public IEnumerable<BranchDailyReport> BranchDaily(Guid branchId)
    {
        if (!_dbContext.BranchDailyReports.Any())
        {
            foreach (var offset in Enumerable.Range(0, 7))
            {
                _dbContext.BranchDailyReports.Add(new BranchDailyReport(DateOnly.FromDateTime(DateTime.Today.AddDays(-offset)), 1000 + offset * 25, 30 + offset, 200 + offset * 10));
            }
            _dbContext.SaveChanges();
        }
        return _dbContext.BranchDailyReports.AsNoTracking().ToList();
    }
}
