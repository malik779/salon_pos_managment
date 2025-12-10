using AuditService.Application.Abstractions;
using AuditService.Domain.Entities;
using MediatR;

namespace AuditService.Application.Audit.Queries.ListAuditEvents;

public sealed record ListAuditEventsQuery() : IRequest<IReadOnlyList<AuditEvent>>;

public sealed class ListAuditEventsQueryHandler : IRequestHandler<ListAuditEventsQuery, IReadOnlyList<AuditEvent>>
{
    private readonly IAuditRepository _repository;

    public ListAuditEventsQueryHandler(IAuditRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<AuditEvent>> Handle(ListAuditEventsQuery request, CancellationToken cancellationToken)
    {
        return _repository.ListAsync(cancellationToken);
    }
}
