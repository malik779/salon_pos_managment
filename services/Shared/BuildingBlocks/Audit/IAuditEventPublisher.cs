namespace Salon.BuildingBlocks.Audit;

public interface IAuditEventPublisher
{
    Task PublishAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default);
}
