using PosService.Application.Abstractions;
using PosService.Domain.Entities;

namespace PosService.Infrastructure.Persistence;

public sealed class InMemoryInvoiceRepository : IInvoiceRepository
{
    private readonly Dictionary<Guid, Invoice> _invoices = new();

    public Task AddAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        _invoices[invoice.Id] = invoice;
        return Task.CompletedTask;
    }

    public Task<Invoice?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        _invoices.TryGetValue(id, out var invoice);
        return Task.FromResult(invoice);
    }

    public Task<IReadOnlyList<Invoice>> ListAsync(Guid branchId, CancellationToken cancellationToken)
    {
        IReadOnlyList<Invoice> snapshot = _invoices.Values.Where(x => x.BranchId == branchId).ToList();
        return Task.FromResult(snapshot);
    }
}
