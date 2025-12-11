using NotificationsService.Application.Abstractions;
using NotificationsService.Domain;
using Salon.BuildingBlocks.Messaging;
using Salon.Shared.Contracts.Events;

namespace NotificationsService.Application.Consumers;

public sealed class InvoicePaidIntegrationEventHandler : IIntegrationEventHandler<InvoicePaidIntegrationEvent>
{
    private readonly INotificationRepository _repository;

    public InvoicePaidIntegrationEventHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(InvoicePaidIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var payload = $"Invoice {integrationEvent.InvoiceId} paid totaling {integrationEvent.Total}";
        var message = new NotificationMessage(Guid.NewGuid(), integrationEvent.ClientId, "email", "invoice-paid", payload);
        await _repository.AddAsync(message, cancellationToken);
    }
}
