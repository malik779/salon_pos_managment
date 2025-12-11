using MediatR;
using PosService.Application.Abstractions;
using Salon.BuildingBlocks.Abstractions;

namespace PosService.Application.Invoices;

public sealed record GetInvoiceByIdQuery(Guid Id) : IQuery<InvoiceDto?>;

public sealed class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto?>
{
    private readonly IInvoiceRepository _repository;

    public GetInvoiceByIdQueryHandler(IInvoiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<InvoiceDto?> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _repository.GetAsync(request.Id, cancellationToken);
        return invoice is null
            ? null
            : new InvoiceDto(invoice.Id, invoice.BranchId, invoice.ClientId, invoice.Total, invoice.Status, invoice.CreatedAtUtc, invoice.PaidAtUtc);
    }
}
