using PosService.Domain;

namespace PosService.Application.Abstractions;

public interface IInvoiceRepository
{
    Task<Invoice?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken);
    /// <summary>
    /// Gets all invoices without pagination
    /// Optimized for read-only queries using AsNoTracking
    /// Note: Consider filtering by date range or status for better performance with large datasets
    /// </summary>
    Task<List<Invoice>> GetAllAsync(string? searchTerm, CancellationToken cancellationToken);
}
