using AuditCompliance.Domain.Audit;
using Microsoft.EntityFrameworkCore;

namespace AuditCompliance.Application.Abstractions;

public interface IAuditDbContext
{
    DbSet<AuditRecord> AuditRecords { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
