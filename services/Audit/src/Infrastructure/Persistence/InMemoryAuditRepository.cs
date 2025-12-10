using AuditService.Application.Abstractions;
using AuditService.Domain.Entities;

namespace AuditService.Infrastructure.Persistence;

public sealed class InMemoryAuditRepository : IAuditRepository
{
    private readonly List<AuditEvent> _events = new();

    public Task AddAsync(AuditEvent auditEvent, CancellationToken cancellationToken)
    {
        _events.Add(auditEvent);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<AuditEvent>> ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<AuditEvent> snapshot = _events.ToList();
        return Task.FromResult(snapshot);
    }
}
