using PosService.Application.Abstractions;
using PosService.Application.Invoices.Models;
using FluentValidation;
using MediatR;

namespace PosService.Application.Invoices.Commands.RefundInvoice;

public sealed record RefundInvoiceCommand(Guid InvoiceId, decimal Amount, string Reason) : IRequest<InvoiceDto?>;

public sealed class RefundInvoiceCommandValidator : AbstractValidator<RefundInvoiceCommand>
{
    public RefundInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public sealed class RefundInvoiceCommandHandler : IRequestHandler<RefundInvoiceCommand, InvoiceDto?>
{
    private readonly IInvoiceRepository _repository;

    public RefundInvoiceCommandHandler(IInvoiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<InvoiceDto?> Handle(RefundInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _repository.GetAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
        {
            return null;
        }

        invoice.Refund();
        return new InvoiceDto(invoice.Id, invoice.BranchId, invoice.ClientId, invoice.Subtotal, invoice.Tax, invoice.Discount, invoice.Total, invoice.Status);
    }
}
