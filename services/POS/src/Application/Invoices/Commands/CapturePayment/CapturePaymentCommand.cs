using PosService.Application.Invoices.Models;
using FluentValidation;
using MediatR;

namespace PosService.Application.Invoices.Commands.CapturePayment;

public sealed record CapturePaymentCommand(Guid InvoiceId, string Method, decimal Amount) : IRequest<PaymentDto>;

public sealed class CapturePaymentCommandValidator : AbstractValidator<CapturePaymentCommand>
{
    public CapturePaymentCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Method).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public sealed class CapturePaymentCommandHandler : IRequestHandler<CapturePaymentCommand, PaymentDto>
{
    public Task<PaymentDto> Handle(CapturePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = new PaymentDto(Guid.NewGuid(), request.InvoiceId, request.Method, request.Amount, "Captured");
        return Task.FromResult(payment);
    }
}
