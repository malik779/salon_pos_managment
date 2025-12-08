using BranchConfiguration.Application.Abstractions;
using BranchConfiguration.Domain.Branches;
using BranchConfiguration.Domain;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BranchConfiguration.Application.Branches.Commands.CreateBranch;

public sealed record CreateBranchCommand(
    string Name,
    string Timezone,
    string Address,
    decimal DefaultTaxRate,
    TimeSpan OpenTimeUtc,
    TimeSpan CloseTimeUtc) : ICommand<Guid>;

public sealed class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Timezone).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DefaultTaxRate).InclusiveBetween(0, 1);
        RuleFor(x => x.OpenTimeUtc).LessThan(x => x.CloseTimeUtc);
    }
}

public sealed class CreateBranchCommandHandler(IBranchConfigurationDbContext context)
    : ICommandHandler<CreateBranchCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.Branches.AnyAsync(b => b.Name == request.Name, cancellationToken);
        if (exists)
        {
            return Result.Failure<Guid>(new Error("BranchConfiguration.Duplicate", "Branch with the same name already exists"));
        }

        var branch = Branch.Create(request.Name, request.Timezone, request.Address, request.DefaultTaxRate, request.OpenTimeUtc, request.CloseTimeUtc);
        context.Branches.Add(branch);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(branch.Id);
    }
}
