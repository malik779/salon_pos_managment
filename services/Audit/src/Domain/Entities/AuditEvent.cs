namespace AuditService.Domain.Entities;

public sealed class AuditEvent
{
    private AuditEvent()
    {
        Actor = string.Empty;
        Action = string.Empty;
        EntityType = string.Empty;
        EntityId = string.Empty;
    }

    public AuditEvent(Guid id, string actor, string action, DateTime timestampUtc, string entityType, string entityId)
    {
        Id = id;
        Actor = actor;
        Action = action;
        TimestampUtc = timestampUtc;
        EntityType = entityType;
        EntityId = entityId;
    }

    public Guid Id { get; private set; }
    public string Actor { get; private set; }
    public string Action { get; private set; }
    public DateTime TimestampUtc { get; private set; }
    public string EntityType { get; private set; }
    public string EntityId { get; private set; }
}
