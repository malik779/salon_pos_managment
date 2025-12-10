using PaymentsService.Application.Abstractions;
using PaymentsService.Domain.Entities;

namespace PaymentsService.Infrastructure.Persistence;

public sealed class InMemoryPaymentIntentRepository : IPaymentIntentRepository
{
    private readonly Dictionary<Guid, PaymentIntent> _intents = new();

    public Task AddAsync(PaymentIntent intent, CancellationToken cancellationToken)
    {
        _intents[intent.Id] = intent;
        return Task.CompletedTask;
    }

    public Task<PaymentIntent?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        _intents.TryGetValue(id, out var intent);
        return Task.FromResult(intent);
    }

    public Task UpdateAsync(PaymentIntent intent, CancellationToken cancellationToken)
    {
        _intents[intent.Id] = intent;
        return Task.CompletedTask;
    }
}
