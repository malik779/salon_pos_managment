using System.Collections.Generic;
using AuditService.Application.Abstractions;
using AuditService.Domain;
using FluentValidation;
using MediatR;
using Salon.BuildingBlocks.Abstractions;
using Salon.BuildingBlocks.Audit;

namespace AuditService.Application.AuditEvents;

public sealed record AuditRecordDto(Guid Id, string Service, string Action, string Actor, string EntityId, string Payload, DateTime OccurredOnUtc);

public sealed record StoreAuditRecordCommand(string Service, string Action, string Actor, string EntityId, Dictionary<string, object?> Metadata)
    : ICommand<AuditRecordDto>, IAuditableRequest
{
    public AuditEvent CreateAuditEvent(object response)
    {
        return new AuditEvent(Service, "AuditStored", Actor, DateTime.UtcNow, Metadata);
    }
}

public sealed class StoreAuditRecordCommandValidator : AbstractValidator<StoreAuditRecordCommand>
{
    public StoreAuditRecordCommandValidator()
    {
        RuleFor(x => x.Service).NotEmpty();
        RuleFor(x => x.Action).NotEmpty();
        RuleFor(x => x.Actor).NotEmpty();
        RuleFor(x => x.EntityId).NotEmpty();
    }
}

public sealed class StoreAuditRecordCommandHandler : IRequestHandler<StoreAuditRecordCommand, AuditRecordDto>
{
    private readonly IAuditRepository _repository;

    public StoreAuditRecordCommandHandler(IAuditRepository repository)
    {
        _repository = repository;
    }

    public async Task<AuditRecordDto> Handle(StoreAuditRecordCommand request, CancellationToken cancellationToken)
    {
        var payload = System.Text.Json.JsonSerializer.Serialize(request.Metadata);
        var record = new AuditRecord(Guid.NewGuid(), request.Service, request.Action, request.Actor, request.EntityId, payload);
        await _repository.AddAsync(record, cancellationToken);
        return new AuditRecordDto(record.Id, record.Service, record.Action, record.Actor, record.EntityId, record.Payload, record.OccurredOnUtc);
    }
}
