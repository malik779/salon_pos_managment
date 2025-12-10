using PosService.Application.Abstractions;
using PosService.Application.Invoices.Models;
using PosService.Domain.Entities;
using FluentValidation;
using MediatR;
using Salon.ServiceDefaults.Messaging;

namespace PosService.Application.Invoices.Commands.CreateInvoice;

public sealed record CreateInvoiceCommand(Guid BranchId, Guid ClientId, List<InvoiceLineDto> Lines, decimal Discount) : IRequest<InvoiceDto>;

public sealed record InvoiceLineDto(Guid ItemId, string ItemType, int Quantity, decimal UnitPrice);

public sealed class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.Lines).NotEmpty();
    }
}

public sealed class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, InvoiceDto>
{
    private readonly IInvoiceRepository _repository;
    private readonly IEventPublisher _eventPublisher;

    public CreateInvoiceCommandHandler(IInvoiceRepository repository, IEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    public async Task<InvoiceDto> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var subtotal = request.Lines.Sum(x => x.UnitPrice * x.Quantity);
        var tax = subtotal * 0.12m;
        var total = subtotal + tax - request.Discount;
        var lines = request.Lines
            .Select(line => new InvoiceLine(line.ItemId, line.ItemType, line.Quantity, line.UnitPrice))
            .ToList();
        var invoice = new Invoice(Guid.NewGuid(), request.BranchId, request.ClientId, subtotal, tax, request.Discount, total, "Draft", lines);
        await _repository.AddAsync(invoice, cancellationToken);

        await _eventPublisher.PublishAsync("pos.invoices", "pos.invoice.created", new { invoice.Id, invoice.Total }, cancellationToken);
        return new InvoiceDto(invoice.Id, invoice.BranchId, invoice.ClientId, subtotal, tax, request.Discount, total, invoice.Status);
    }
}
