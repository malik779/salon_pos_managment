using MediatR;
using PaymentsService.Application.Abstractions;
using Salon.BuildingBlocks.Abstractions;

namespace PaymentsService.Application.PaymentIntents;

public sealed record GetPaymentIntentByIdQuery(Guid Id) : IQuery<PaymentIntentDto?>;

public sealed class GetPaymentIntentByIdQueryHandler : IRequestHandler<GetPaymentIntentByIdQuery, PaymentIntentDto?>
{
    private readonly IPaymentIntentRepository _repository;

    public GetPaymentIntentByIdQueryHandler(IPaymentIntentRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaymentIntentDto?> Handle(GetPaymentIntentByIdQuery request, CancellationToken cancellationToken)
    {
        var intent = await _repository.GetAsync(request.Id, cancellationToken);
        return intent is null
            ? null
            : new PaymentIntentDto(intent.Id, intent.InvoiceId, intent.Amount, intent.Provider, intent.Status, intent.CreatedAtUtc);
    }
}
