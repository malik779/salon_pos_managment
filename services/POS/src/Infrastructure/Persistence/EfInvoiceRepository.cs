using Microsoft.EntityFrameworkCore;
using PosService.Application.Abstractions;
using PosService.Domain.Entities;

namespace PosService.Infrastructure.Persistence;

public sealed class EfInvoiceRepository : IInvoiceRepository
{
    private readonly PosDbContext _dbContext;

    public EfInvoiceRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        await _dbContext.Invoices.AddAsync(invoice, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Invoice?> GetAsync(Guid id, CancellationToken cancellationToken)
        => _dbContext.Invoices.Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Invoice>> ListAsync(Guid branchId, CancellationToken cancellationToken)
    {
        return await _dbContext.Invoices.AsNoTracking().Where(x => x.BranchId == branchId).ToListAsync(cancellationToken);
    }
}
