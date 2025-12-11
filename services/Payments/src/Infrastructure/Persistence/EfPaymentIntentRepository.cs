using Microsoft.EntityFrameworkCore;
using PaymentsService.Application.Abstractions;
using PaymentsService.Domain.Entities;

namespace PaymentsService.Infrastructure.Persistence;

public sealed class EfPaymentIntentRepository : IPaymentIntentRepository
{
    private readonly PaymentsDbContext _dbContext;

    public EfPaymentIntentRepository(PaymentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(PaymentIntent intent, CancellationToken cancellationToken)
    {
        await _dbContext.PaymentIntents.AddAsync(intent, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<PaymentIntent?> GetAsync(Guid id, CancellationToken cancellationToken)
        => _dbContext.PaymentIntents.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task UpdateAsync(PaymentIntent intent, CancellationToken cancellationToken)
    {
        _dbContext.PaymentIntents.Update(intent);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
