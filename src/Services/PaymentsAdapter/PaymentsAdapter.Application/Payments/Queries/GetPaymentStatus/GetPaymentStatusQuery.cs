using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using PaymentsAdapter.Application.Abstractions;

namespace PaymentsAdapter.Application.Payments.Queries.GetPaymentStatus;

public sealed record PaymentStatusDto(Guid Id, Guid InvoiceId, decimal Amount, string Method, string Status, DateTime? CapturedAtUtc);

public sealed record GetPaymentStatusQuery(Guid PaymentId) : IQuery<PaymentStatusDto>;

public sealed class GetPaymentStatusQueryHandler(IPaymentsDbContext context)
    : IQueryHandler<GetPaymentStatusQuery, PaymentStatusDto>
{
    public async Task<Result<PaymentStatusDto>> Handle(GetPaymentStatusQuery request, CancellationToken cancellationToken)
    {
        var payment = await context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PaymentId, cancellationToken);

        if (payment is null)
        {
            return Result.Failure<PaymentStatusDto>(new Error("PaymentsAdapter.PaymentNotFound", "Payment not found"));
        }

        var dto = new PaymentStatusDto(payment.Id, payment.InvoiceId, payment.Amount, payment.Method, payment.Status, payment.CapturedAtUtc);
        return Result.Success(dto);
    }
}
