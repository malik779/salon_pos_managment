using AuditCompliance.Application.Abstractions;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace AuditCompliance.Application.Audit.Queries.GetAuditTrail;

public sealed record AuditRecordDto(Guid Id, string Category, string Actor, string Payload, DateTime OccurredOnUtc);

public sealed record GetAuditTrailQuery(string Category, DateTime FromUtc, DateTime ToUtc) : IQuery<IReadOnlyCollection<AuditRecordDto>>;

public sealed class GetAuditTrailQueryHandler(IAuditDbContext context)
    : IQueryHandler<GetAuditTrailQuery, IReadOnlyCollection<AuditRecordDto>>
{
    public async Task<Result<IReadOnlyCollection<AuditRecordDto>>> Handle(GetAuditTrailQuery request, CancellationToken cancellationToken)
    {
        var records = await context.AuditRecords
            .AsNoTracking()
            .Where(r => r.Category == request.Category && r.OccurredOnUtc >= request.FromUtc && r.OccurredOnUtc <= request.ToUtc)
            .OrderByDescending(r => r.OccurredOnUtc)
            .Select(r => new AuditRecordDto(r.Id, r.Category, r.Actor, r.Payload, r.OccurredOnUtc))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyCollection<AuditRecordDto>>(records);
    }
}
