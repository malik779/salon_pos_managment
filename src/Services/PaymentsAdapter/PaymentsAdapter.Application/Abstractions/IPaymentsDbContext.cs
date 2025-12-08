using Microsoft.EntityFrameworkCore;
using PaymentsAdapter.Domain.Payments;

namespace PaymentsAdapter.Application.Abstractions;

public interface IPaymentsDbContext
{
    DbSet<Payment> Payments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
