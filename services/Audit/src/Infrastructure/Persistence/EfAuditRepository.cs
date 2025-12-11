using AuditService.Application.Abstractions;
using AuditService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuditService.Infrastructure.Persistence;

public sealed class EfAuditRepository : IAuditRepository
{
    private readonly AuditDbContext _dbContext;

    public EfAuditRepository(AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(AuditEvent auditEvent, CancellationToken cancellationToken)
    {
        await _dbContext.AuditEvents.AddAsync(auditEvent, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AuditEvent>> ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.AuditEvents.AsNoTracking().OrderByDescending(x => x.TimestampUtc).ToListAsync(cancellationToken);
    }
}
