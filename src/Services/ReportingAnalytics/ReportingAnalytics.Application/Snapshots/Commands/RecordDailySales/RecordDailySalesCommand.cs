using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ReportingAnalytics.Application.Abstractions;
using ReportingAnalytics.Domain.Snapshots;

namespace ReportingAnalytics.Application.Snapshots.Commands.RecordDailySales;

public sealed record RecordDailySalesCommand(
    Guid BranchId,
    DateOnly BusinessDate,
    decimal TotalSales,
    decimal Tax,
    decimal Discount,
    int Invoices) : ICommand<Guid>;

public sealed class RecordDailySalesCommandValidator : AbstractValidator<RecordDailySalesCommand>
{
    public RecordDailySalesCommandValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.BusinessDate).NotEmpty();
        RuleFor(x => x.Invoices).GreaterThanOrEqualTo(0);
    }
}

public sealed class RecordDailySalesCommandHandler(IReportingDbContext context)
    : ICommandHandler<RecordDailySalesCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RecordDailySalesCommand request, CancellationToken cancellationToken)
    {
        var existing = await context.DailySales.AnyAsync(ds =>
            ds.BranchId == request.BranchId && ds.BusinessDate == request.BusinessDate, cancellationToken);
        if (existing)
        {
            return Result.Failure<Guid>(new Error("ReportingAnalytics.Duplicate", "Snapshot already recorded for this day"));
        }

        var snapshot = DailySalesSnapshot.Record(request.BranchId, request.BusinessDate, request.TotalSales, request.Tax, request.Discount, request.Invoices);
        context.DailySales.Add(snapshot);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(snapshot.Id);
    }
}
