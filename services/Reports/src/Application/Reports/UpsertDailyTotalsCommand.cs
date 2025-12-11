using System.Collections.Generic;
using FluentValidation;
using MediatR;
using ReportsService.Application.Abstractions;
using ReportsService.Domain;
using Salon.BuildingBlocks.Abstractions;
using Salon.BuildingBlocks.Audit;

namespace ReportsService.Application.Reports;

public sealed record ReportSnapshotDto(Guid BranchId, DateOnly Day, decimal TotalRevenue, DateTime GeneratedAtUtc);

public sealed record UpsertDailyTotalsCommand(Guid BranchId, DateOnly Day, decimal TotalRevenue, string Actor)
    : ICommand<ReportSnapshotDto>, IAuditableRequest
{
    public AuditEvent CreateAuditEvent(object response)
    {
        return new AuditEvent(
            Service: "reports-service",
            Action: "DailyTotalsUpserted",
            Actor: Actor,
            OccurredOnUtc: DateTime.UtcNow,
            Metadata: new Dictionary<string, object?>
            {
                ["branchId"] = BranchId,
                ["day"] = Day,
                ["total"] = TotalRevenue
            });
    }
}

public sealed class UpsertDailyTotalsCommandValidator : AbstractValidator<UpsertDailyTotalsCommand>
{
    public UpsertDailyTotalsCommandValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.TotalRevenue).GreaterThanOrEqualTo(0);
    }
}

public sealed class UpsertDailyTotalsCommandHandler : IRequestHandler<UpsertDailyTotalsCommand, ReportSnapshotDto>
{
    private readonly IReportRepository _repository;

    public UpsertDailyTotalsCommandHandler(IReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<ReportSnapshotDto> Handle(UpsertDailyTotalsCommand request, CancellationToken cancellationToken)
    {
        var snapshot = await _repository.GetAsync(request.BranchId, request.Day, cancellationToken)
                       ?? new ReportSnapshot(Guid.NewGuid(), request.BranchId, request.Day, request.TotalRevenue);

        snapshot.Update(request.TotalRevenue);
        await _repository.UpsertAsync(snapshot, cancellationToken);

        return new ReportSnapshotDto(snapshot.BranchId, snapshot.Day, snapshot.TotalRevenue, snapshot.GeneratedAtUtc);
    }
}
