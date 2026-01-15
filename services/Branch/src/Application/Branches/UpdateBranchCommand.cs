using System.Collections.Generic;
using BranchService.Application.Abstractions;
using BranchService.Domain;
using FluentValidation;
using MediatR;
using Salon.BuildingBlocks.Abstractions;
using Salon.BuildingBlocks.Audit;

namespace BranchService.Application.Branches;

public sealed record UpdateBranchCommand(Guid Id, string Name, string Timezone, string Address, string Actor)
    : ICommand<Unit>, IAuditableRequest
{
    public AuditEvent CreateAuditEvent(object response)
    {
        return new AuditEvent(
            Service: "branch-service",
            Action: "BranchUpdated",
            Actor: Actor,
            OccurredOnUtc: DateTime.UtcNow,
            Metadata: new Dictionary<string, object?>
            {
                ["branchId"] = Id,
                ["name"] = Name,
                ["timezone"] = Timezone,
                ["address"] = Address
            });
    }
}

public sealed class UpdateBranchCommandValidator : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Timezone).NotEmpty();
        RuleFor(x => x.Address).NotEmpty();
    }
}

public sealed class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, Unit>
{
    private readonly IBranchRepository _repository;

    public UpdateBranchCommandHandler(IBranchRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await _repository.GetAsync(request.Id, cancellationToken);
        if (branch is null)
        {
            throw new InvalidOperationException($"Branch with id {request.Id} not found");
        }

        branch.UpdateDetails(request.Name, request.Timezone, request.Address);
        await _repository.UpdateAsync(branch, cancellationToken);

        return Unit.Value;
    }
}
