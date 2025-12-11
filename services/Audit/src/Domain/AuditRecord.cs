namespace AuditService.Domain;

public class AuditRecord
{
    private AuditRecord()
    {
        Service = string.Empty;
        Action = string.Empty;
        Actor = string.Empty;
        EntityId = string.Empty;
    }

    public AuditRecord(Guid id, string service, string action, string actor, string entityId, string payload)
    {
        Id = id;
        Service = service;
        Action = action;
        Actor = actor;
        EntityId = entityId;
        Payload = payload;
        OccurredOnUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Service { get; private set; }
    public string Action { get; private set; }
    public string Actor { get; private set; }
    public string EntityId { get; private set; }
    public string Payload { get; private set; } = string.Empty;
    public DateTime OccurredOnUtc { get; private set; }
}
