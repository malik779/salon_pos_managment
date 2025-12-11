using System.Collections.Generic;
using FluentValidation;
using MediatR;
using PosService.Application.Abstractions;
using PosService.Domain;
using Salon.BuildingBlocks.Abstractions;
using Salon.BuildingBlocks.Audit;
using Salon.BuildingBlocks.Messaging;
using Salon.Shared.Contracts.Events;

namespace PosService.Application.Invoices;

public sealed record InvoiceDto(Guid Id, Guid BranchId, Guid ClientId, decimal Total, string Status, DateTime CreatedAtUtc, DateTime? PaidAtUtc);

public sealed record FinalizeInvoiceCommand(Guid BranchId, Guid ClientId, decimal Total, string Actor)
    : ICommand<InvoiceDto>, IAuditableRequest, IRetryableRequest
{
    public AuditEvent CreateAuditEvent(object response)
    {
        return new AuditEvent(
            Service: "pos-service",
            Action: "InvoiceFinalized",
            Actor: Actor,
            OccurredOnUtc: DateTime.UtcNow,
            Metadata: new Dictionary<string, object?>
            {
                ["branchId"] = BranchId,
                ["clientId"] = ClientId,
                ["total"] = Total
            });
    }

    public int RetryCount => 3;
}

public sealed class FinalizeInvoiceCommandValidator : AbstractValidator<FinalizeInvoiceCommand>
{
    public FinalizeInvoiceCommandValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.Total).GreaterThan(0);
    }
}

public sealed class FinalizeInvoiceCommandHandler : IRequestHandler<FinalizeInvoiceCommand, InvoiceDto>
{
    private readonly IInvoiceRepository _repository;
    private readonly IIntegrationEventPublisher _publisher;

    public FinalizeInvoiceCommandHandler(IInvoiceRepository repository, IIntegrationEventPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<InvoiceDto> Handle(FinalizeInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = new Invoice(Guid.NewGuid(), request.BranchId, request.ClientId, request.Total);
        invoice.MarkPaid();
        await _repository.AddAsync(invoice, cancellationToken);

        var integrationEvent = new InvoicePaidIntegrationEvent(invoice.Id, invoice.BranchId, invoice.ClientId, invoice.Total, invoice.PaidAtUtc ?? DateTime.UtcNow);
        await _publisher.PublishAsync(integrationEvent, cancellationToken);

        return new InvoiceDto(invoice.Id, invoice.BranchId, invoice.ClientId, invoice.Total, invoice.Status, invoice.CreatedAtUtc, invoice.PaidAtUtc);
    }
}
