using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using ReportingAnalytics.Application.Abstractions;

namespace ReportingAnalytics.Application.Snapshots.Queries.GetDailySales;

public sealed record DailySalesDto(
    Guid Id,
    Guid BranchId,
    DateOnly BusinessDate,
    decimal TotalSales,
    decimal Tax,
    decimal Discount,
    int Invoices);

public sealed record GetDailySalesQuery(Guid BranchId, DateOnly BusinessDate) : IQuery<DailySalesDto>;

public sealed class GetDailySalesQueryHandler(IReportingDbContext context)
    : IQueryHandler<GetDailySalesQuery, DailySalesDto>
{
    public async Task<Result<DailySalesDto>> Handle(GetDailySalesQuery request, CancellationToken cancellationToken)
    {
        var snapshot = await context.DailySales
            .AsNoTracking()
            .FirstOrDefaultAsync(ds => ds.BranchId == request.BranchId && ds.BusinessDate == request.BusinessDate, cancellationToken);

        if (snapshot is null)
        {
            return Result.Failure<DailySalesDto>(new Error("ReportingAnalytics.SnapshotNotFound", "Snapshot not found"));
        }

        var dto = new DailySalesDto(snapshot.Id, snapshot.BranchId, snapshot.BusinessDate, snapshot.TotalSales, snapshot.Tax, snapshot.Discount, snapshot.Invoices);
        return Result.Success(dto);
    }
}
