using PaymentsService.Domain;

namespace PaymentsService.Application.Abstractions;

public interface IPaymentIntentRepository
{
    Task<PaymentIntent?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(PaymentIntent intent, CancellationToken cancellationToken);
}
