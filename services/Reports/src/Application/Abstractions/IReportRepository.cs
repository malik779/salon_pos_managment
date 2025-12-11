using ReportsService.Domain;

namespace ReportsService.Application.Abstractions;

public interface IReportRepository
{
    Task<ReportSnapshot?> GetAsync(Guid branchId, DateOnly day, CancellationToken cancellationToken);
    Task UpsertAsync(ReportSnapshot snapshot, CancellationToken cancellationToken);
}
