using MediatR;
using PosService.Application.Abstractions;
using Salon.BuildingBlocks.Abstractions;

namespace PosService.Application.Invoices;

/// <summary>
/// Query to fetch all invoices without pagination
/// Optimized for dropdowns, selects, and other UI components that need complete lists
/// Performance: Uses AsNoTracking for read-only queries to reduce memory overhead
/// Note: Consider filtering by date range or status for better performance with large datasets
/// </summary>
public sealed record GetAllInvoicesQuery(string? SearchTerm = null) : IQuery<List<InvoiceDto>>;

public sealed class GetAllInvoicesQueryHandler : IRequestHandler<GetAllInvoicesQuery, List<InvoiceDto>>
{
    private readonly IInvoiceRepository _repository;

    public GetAllInvoicesQueryHandler(IInvoiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<InvoiceDto>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _repository.GetAllAsync(request.SearchTerm, cancellationToken);
        return invoices.Select(i => new InvoiceDto(
            i.Id,
            i.BranchId,
            i.ClientId,
            i.Total,
            i.Status,
            i.CreatedAtUtc,
            i.PaidAtUtc)).ToList();
    }
}
