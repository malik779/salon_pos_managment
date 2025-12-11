using AuditService.Domain;

namespace AuditService.Application.Abstractions;

public interface IAuditRepository
{
    Task<AuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(AuditRecord record, CancellationToken cancellationToken);
}
