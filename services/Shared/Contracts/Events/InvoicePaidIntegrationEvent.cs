using Salon.BuildingBlocks.Messaging;

namespace Salon.Shared.Contracts.Events;

public sealed record InvoicePaidIntegrationEvent(Guid InvoiceId, Guid BranchId, Guid ClientId, decimal Total, DateTime PaidAtUtc)
    : IntegrationEvent;
