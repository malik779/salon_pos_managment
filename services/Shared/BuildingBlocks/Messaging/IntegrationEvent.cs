namespace Salon.BuildingBlocks.Messaging;

public abstract record IntegrationEvent(Guid EventId, DateTime OccurredOnUtc)
{
    protected IntegrationEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }
}
