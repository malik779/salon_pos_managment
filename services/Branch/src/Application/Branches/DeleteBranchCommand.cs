using System.Collections.Generic;
using BranchService.Application.Abstractions;
using FluentValidation;
using MediatR;
using Salon.BuildingBlocks.Abstractions;
using Salon.BuildingBlocks.Audit;

namespace BranchService.Application.Branches;

public sealed record DeleteBranchCommand(Guid Id, string Actor)
    : ICommand<Unit>, IAuditableRequest
{
    public AuditEvent CreateAuditEvent(object response)
    {
        return new AuditEvent(
            Service: "branch-service",
            Action: "BranchDeleted",
            Actor: Actor,
            OccurredOnUtc: DateTime.UtcNow,
            Metadata: new Dictionary<string, object?>
            {
                ["branchId"] = Id
            });
    }
}

public sealed class DeleteBranchCommandValidator : AbstractValidator<DeleteBranchCommand>
{
    public DeleteBranchCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public sealed class DeleteBranchCommandHandler : IRequestHandler<DeleteBranchCommand, Unit>
{
    private readonly IBranchRepository _repository;

    public DeleteBranchCommandHandler(IBranchRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await _repository.GetAsync(request.Id, cancellationToken);
        if (branch is null)
        {
            throw new InvalidOperationException($"Branch with id {request.Id} not found");
        }

        await _repository.DeleteAsync(branch, cancellationToken);

        return Unit.Value;
    }
}
