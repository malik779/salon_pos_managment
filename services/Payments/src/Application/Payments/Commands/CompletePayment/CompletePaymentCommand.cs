using MediatR;
using PaymentsService.Application.Abstractions;
using PaymentsService.Application.Payments.Models;

namespace PaymentsService.Application.Payments.Commands.CompletePayment;

public sealed record CompletePaymentCommand(Guid PaymentId) : IRequest<PaymentIntentDto?>;

public sealed class CompletePaymentCommandHandler : IRequestHandler<CompletePaymentCommand, PaymentIntentDto?>
{
    private readonly IPaymentIntentRepository _repository;

    public CompletePaymentCommandHandler(IPaymentIntentRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaymentIntentDto?> Handle(CompletePaymentCommand request, CancellationToken cancellationToken)
    {
        var intent = await _repository.GetAsync(request.PaymentId, cancellationToken);
        if (intent is null)
        {
            return null;
        }

        intent.MarkCaptured();
        await _repository.UpdateAsync(intent, cancellationToken);
        return new PaymentIntentDto(intent.Id, intent.InvoiceId, intent.Amount, intent.Status, intent.ProviderReference);
    }
}
