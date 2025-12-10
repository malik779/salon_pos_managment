using AuditService.Domain.Entities;

namespace AuditService.Application.Abstractions;

public interface IAuditRepository
{
    Task AddAsync(AuditEvent auditEvent, CancellationToken cancellationToken);
    Task<IReadOnlyList<AuditEvent>> ListAsync(CancellationToken cancellationToken);
}
