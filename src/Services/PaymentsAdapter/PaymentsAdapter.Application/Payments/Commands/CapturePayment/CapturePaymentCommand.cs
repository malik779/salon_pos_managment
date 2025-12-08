using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PaymentsAdapter.Application.Abstractions;
using PaymentsAdapter.Domain.Payments;

namespace PaymentsAdapter.Application.Payments.Commands.CapturePayment;

public sealed record CapturePaymentCommand(Guid InvoiceId, decimal Amount, string Method) : ICommand<Guid>;

public sealed class CapturePaymentCommandValidator : AbstractValidator<CapturePaymentCommand>
{
    public CapturePaymentCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Method).NotEmpty().MaximumLength(50);
    }
}

public sealed class CapturePaymentCommandHandler(IPaymentsDbContext context)
    : ICommandHandler<CapturePaymentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CapturePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = Payment.Capture(request.InvoiceId, request.Amount, request.Method);
        context.Payments.Add(payment);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(payment.Id);
    }
}
