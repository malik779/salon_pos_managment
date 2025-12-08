using AuditCompliance.Application.Abstractions;
using AuditCompliance.Domain.Audit;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using FluentValidation;

namespace AuditCompliance.Application.Audit.Commands.RecordAuditEvent;

public sealed record RecordAuditEventCommand(string Category, string Actor, string Payload) : ICommand<Guid>;

public sealed class RecordAuditEventCommandValidator : AbstractValidator<RecordAuditEventCommand>
{
    public RecordAuditEventCommandValidator()
    {
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Actor).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Payload).NotEmpty();
    }
}

public sealed class RecordAuditEventCommandHandler(IAuditDbContext context)
    : ICommandHandler<RecordAuditEventCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RecordAuditEventCommand request, CancellationToken cancellationToken)
    {
        var record = AuditRecord.Create(request.Category, request.Actor, request.Payload);
        context.AuditRecords.Add(record);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(record.Id);
    }
}
