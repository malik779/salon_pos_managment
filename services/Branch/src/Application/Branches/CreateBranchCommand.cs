using System.Collections.Generic;
using BranchService.Application.Abstractions;
using BranchService.Domain;
using FluentValidation;
using MediatR;
using Salon.BuildingBlocks.Abstractions;
using Salon.BuildingBlocks.Audit;

namespace BranchService.Application.Branches;

public sealed record BranchDto(Guid Id, string Name, string Timezone, string Address);

public sealed record CreateBranchCommand(string Name, string Timezone, string Address, string Actor)
    : ICommand<BranchDto>, IAuditableRequest
{
    public AuditEvent CreateAuditEvent(object response)
    {
        return new AuditEvent(
            Service: "branch-service",
            Action: "BranchCreated",
            Actor: Actor,
            OccurredOnUtc: DateTime.UtcNow,
            Metadata: new Dictionary<string, object?>
            {
                ["name"] = Name,
                ["timezone"] = Timezone,
                ["address"] = Address
            });
    }
}

public sealed class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Timezone).NotEmpty();
        RuleFor(x => x.Address).NotEmpty();
    }
}

public sealed class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, BranchDto>
{
    private readonly IBranchRepository _repository;

    public CreateBranchCommandHandler(IBranchRepository repository)
    {
        _repository = repository;
    }

    public async Task<BranchDto> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = new Branch(Guid.NewGuid(), request.Name, request.Timezone, request.Address);
        await _repository.AddAsync(branch, cancellationToken);
        return new BranchDto(branch.Id, branch.Name, branch.Timezone, branch.Address);
    }
}
