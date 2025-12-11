using AuditService.Application.Abstractions;
using MediatR;
using Salon.BuildingBlocks.Abstractions;

namespace AuditService.Application.AuditEvents;

public sealed record GetAuditRecordQuery(Guid Id) : IQuery<AuditRecordDto?>;

public sealed class GetAuditRecordQueryHandler : IRequestHandler<GetAuditRecordQuery, AuditRecordDto?>
{
    private readonly IAuditRepository _repository;

    public GetAuditRecordQueryHandler(IAuditRepository repository)
    {
        _repository = repository;
    }

    public async Task<AuditRecordDto?> Handle(GetAuditRecordQuery request, CancellationToken cancellationToken)
    {
        var record = await _repository.GetAsync(request.Id, cancellationToken);
        return record is null
            ? null
            : new AuditRecordDto(record.Id, record.Service, record.Action, record.Actor, record.EntityId, record.Payload, record.OccurredOnUtc);
    }
}
