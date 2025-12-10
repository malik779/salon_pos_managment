using PosService.Application.Abstractions;
using PosService.Application.Invoices.Models;
using FluentValidation;
using MediatR;

namespace PosService.Application.Invoices.Commands.FinalizeInvoice;

public sealed record FinalizeInvoiceCommand(Guid InvoiceId) : IRequest<InvoiceDto?>;

public sealed class FinalizeInvoiceCommandValidator : AbstractValidator<FinalizeInvoiceCommand>
{
    public FinalizeInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
    }
}

public sealed class FinalizeInvoiceCommandHandler : IRequestHandler<FinalizeInvoiceCommand, InvoiceDto?>
{
    private readonly IInvoiceRepository _repository;

    public FinalizeInvoiceCommandHandler(IInvoiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<InvoiceDto?> Handle(FinalizeInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _repository.GetAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
        {
            return null;
        }

        invoice.FinalizeInvoice();
        return new InvoiceDto(invoice.Id, invoice.BranchId, invoice.ClientId, invoice.Subtotal, invoice.Tax, invoice.Discount, invoice.Total, invoice.Status);
    }
}
