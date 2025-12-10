using PosService.Domain.Entities;

namespace PosService.Application.Abstractions;

public interface IInvoiceRepository
{
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken);
    Task<Invoice?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Invoice>> ListAsync(Guid branchId, CancellationToken cancellationToken);
}
