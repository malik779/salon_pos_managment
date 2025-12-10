namespace AuditService.Domain.Entities;

public sealed class AuditEvent
{
    public AuditEvent(Guid id, string Actor, string Action, DateTime TimestampUtc, string EntityType, string EntityId)
    {
        Id = id;
        this.Actor = Actor;
        this.Action = Action;
        this.TimestampUtc = TimestampUtc;
        this.EntityType = EntityType;
        this.EntityId = EntityId;
    }

    public Guid Id { get; }
    public string Actor { get; }
    public string Action { get; }
    public DateTime TimestampUtc { get; }
    public string EntityType { get; }
    public string EntityId { get; }
}
