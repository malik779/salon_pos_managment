using Microsoft.EntityFrameworkCore;
using PosTransactions.Domain.Invoices;

namespace PosTransactions.Application.Abstractions;

public interface IPosDbContext
{
    DbSet<Invoice> Invoices { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
