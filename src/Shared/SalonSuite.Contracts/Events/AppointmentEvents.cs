using BuildingBlocks.Messaging.Events;

namespace SalonSuite.Contracts.Events;

public sealed record AppointmentCreatedIntegrationEvent(
    Guid AppointmentId,
    Guid BranchId,
    Guid ClientId,
    Guid StaffId,
    DateTime StartUtc,
    DateTime EndUtc,
    string Status,
    string? CorrelationId = null) : IntegrationEvent(CorrelationId);

public sealed record AppointmentCancelledIntegrationEvent(
    Guid AppointmentId,
    Guid BranchId,
    string Reason,
    string? CorrelationId = null) : IntegrationEvent(CorrelationId);

public sealed record InvoicePaidIntegrationEvent(
    Guid InvoiceId,
    Guid BranchId,
    Guid ClientId,
    decimal Amount,
    string PaymentMethod,
    string? CorrelationId = null) : IntegrationEvent(CorrelationId);
