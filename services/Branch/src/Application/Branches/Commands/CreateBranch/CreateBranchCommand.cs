using BranchService.Application.Abstractions;
using BranchService.Application.Branches.Models;
using BranchService.Domain.Entities;
using FluentValidation;
using MediatR;

namespace BranchService.Application.Branches.Commands.CreateBranch;

public sealed record CreateBranchCommand(string Name, string Timezone, string Address) : IRequest<BranchDto>;

public sealed class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Timezone).NotEmpty();
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
