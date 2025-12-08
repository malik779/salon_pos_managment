using MediatR;

namespace BuildingBlocks.Domain.Abstractions;

public abstract record DomainEvent(Guid EventId, DateTime OccurredOnUtc, string? CorrelationId = null) : INotification
{
    protected DomainEvent(string? correlationId = null)
        : this(Guid.NewGuid(), DateTime.UtcNow, correlationId)
    {
    }
}
