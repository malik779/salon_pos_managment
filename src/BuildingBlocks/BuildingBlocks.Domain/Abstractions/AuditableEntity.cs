namespace BuildingBlocks.Domain.Abstractions;

public abstract class AuditableEntity : Entity
{
    public string CreatedBy { get; protected set; } = default!;
    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
    public string? LastModifiedBy { get; protected set; }
        = null;
    public DateTime? LastModifiedOnUtc { get; set; }
        = null;
}
