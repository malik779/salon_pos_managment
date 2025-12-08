using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using PosTransactions.Application.Abstractions;
using PosTransactions.Domain;

namespace PosTransactions.Application.Invoices.Queries.GetInvoice;

public sealed record InvoiceDetails(
    Guid Id,
    Guid BranchId,
    Guid ClientId,
    decimal Amount,
    decimal Tax,
    decimal Discount,
    string Status,
    DateTime CreatedAtUtc);

public sealed record GetInvoiceQuery(Guid InvoiceId) : IQuery<InvoiceDetails>;

public sealed class GetInvoiceQueryHandler(IPosDbContext context)
    : IQueryHandler<GetInvoiceQuery, InvoiceDetails>
{
    public async Task<Result<InvoiceDetails>> Handle(GetInvoiceQuery request, CancellationToken cancellationToken)
    {
        var invoice = await context.Invoices
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

        if (invoice is null)
        {
            return Result.Failure<InvoiceDetails>(PosErrors.InvoiceNotFound);
        }

        var dto = new InvoiceDetails(invoice.Id, invoice.BranchId, invoice.ClientId, invoice.Amount, invoice.Tax, invoice.Discount, invoice.Status, invoice.CreatedAtUtc);
        return Result.Success(dto);
    }
}
