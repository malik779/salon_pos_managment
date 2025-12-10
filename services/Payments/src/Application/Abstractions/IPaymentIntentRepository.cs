using PaymentsService.Domain.Entities;

namespace PaymentsService.Application.Abstractions;

public interface IPaymentIntentRepository
{
    Task AddAsync(PaymentIntent intent, CancellationToken cancellationToken);
    Task<PaymentIntent?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(PaymentIntent intent, CancellationToken cancellationToken);
}
