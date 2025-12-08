using BuildingBlocks.Domain.Abstractions;

namespace AuditCompliance.Domain.Audit;

public sealed class AuditRecord : AuditableEntity, IAggregateRoot
{
    private AuditRecord()
    {
    }

    private AuditRecord(Guid id, string category, string actor, string payload)
    {
        Id = id;
        Category = category;
        Actor = actor;
        Payload = payload;
        OccurredOnUtc = DateTime.UtcNow;
    }

    public string Category { get; private set; } = default!;
    public string Actor { get; private set; } = default!;
    public string Payload { get; private set; } = default!;
    public DateTime OccurredOnUtc { get; private set; } = DateTime.UtcNow;

    public static AuditRecord Create(string category, string actor, string payload)
        => new(Guid.NewGuid(), category, actor, payload);
}
