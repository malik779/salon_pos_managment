namespace BuildingBlocks.Messaging.Events;

public abstract record IntegrationEvent(string? correlationId = null)
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
    public string? CorrelationId { get; init; } = correlationId;
}
