using AuditService.Application.Abstractions;
using AuditService.Domain.Entities;
using MediatR;

namespace AuditService.Application.Audit.Commands.CreateAuditEvent;

public sealed record CreateAuditEventCommand(string Actor, string Action, string EntityType, string EntityId) : IRequest<AuditEvent>;

public sealed class CreateAuditEventCommandHandler : IRequestHandler<CreateAuditEventCommand, AuditEvent>
{
    private readonly IAuditRepository _repository;

    public CreateAuditEventCommandHandler(IAuditRepository repository)
    {
        _repository = repository;
    }

    public async Task<AuditEvent> Handle(CreateAuditEventCommand request, CancellationToken cancellationToken)
    {
        var audit = new AuditEvent(Guid.NewGuid(), request.Actor, request.Action, DateTime.UtcNow, request.EntityType, request.EntityId);
        await _repository.AddAsync(audit, cancellationToken);
        return audit;
    }
}
