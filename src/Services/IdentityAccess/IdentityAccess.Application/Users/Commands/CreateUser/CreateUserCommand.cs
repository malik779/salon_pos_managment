using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using FluentValidation;
using IdentityAccess.Application.Abstractions;
using IdentityAccess.Domain;
using IdentityAccess.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace IdentityAccess.Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand(string Email, string Name, Guid DefaultBranchId) : ICommand<Guid>;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DefaultBranchId).NotEmpty();
    }
}

public sealed class CreateUserCommandHandler(IIdentityAccessDbContext context)
    : ICommandHandler<CreateUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
        if (exists)
        {
            return Result.Failure<Guid>(IdentityErrors.DuplicateEmail);
        }

        var user = User.Create(request.Email, request.Name, request.DefaultBranchId);
        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(user.Id);
    }
}
