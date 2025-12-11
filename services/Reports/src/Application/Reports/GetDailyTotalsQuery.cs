using MediatR;
using ReportsService.Application.Abstractions;
using Salon.BuildingBlocks.Abstractions;

namespace ReportsService.Application.Reports;

public sealed record GetDailyTotalsQuery(Guid BranchId, DateOnly Day) : IQuery<ReportSnapshotDto?>;

public sealed class GetDailyTotalsQueryHandler : IRequestHandler<GetDailyTotalsQuery, ReportSnapshotDto?>
{
    private readonly IReportRepository _repository;

    public GetDailyTotalsQueryHandler(IReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<ReportSnapshotDto?> Handle(GetDailyTotalsQuery request, CancellationToken cancellationToken)
    {
        var snapshot = await _repository.GetAsync(request.BranchId, request.Day, cancellationToken);
        return snapshot is null
            ? null
            : new ReportSnapshotDto(snapshot.BranchId, snapshot.Day, snapshot.TotalRevenue, snapshot.GeneratedAtUtc);
    }
}
