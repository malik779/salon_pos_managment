using Microsoft.EntityFrameworkCore;
using ReportingAnalytics.Domain.Snapshots;

namespace ReportingAnalytics.Application.Abstractions;

public interface IReportingDbContext
{
    DbSet<DailySalesSnapshot> DailySales { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
