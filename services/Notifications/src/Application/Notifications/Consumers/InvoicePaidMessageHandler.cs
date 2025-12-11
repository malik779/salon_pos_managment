using Microsoft.Extensions.Logging;
using NotificationsService.Application.Notifications.Models;
using Salon.BuildingBlocks.Messaging;

namespace NotificationsService.Application.Notifications.Consumers;

public sealed record InvoicePaidMessage(Guid Id, Guid InvoiceId, decimal Total);

public sealed class InvoicePaidMessageHandler : IRabbitMqMessageHandler<InvoicePaidMessage>
{
    private readonly ILogger<InvoicePaidMessageHandler> _logger;

    public InvoicePaidMessageHandler(ILogger<InvoicePaidMessageHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(InvoicePaidMessage message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Invoice {InvoiceId} paid. Total {Total}", message.InvoiceId, message.Total);
        return Task.CompletedTask;
    }
}
