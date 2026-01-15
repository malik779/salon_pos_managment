using System.Collections.Generic;
using FluentValidation;
using IdentityService.Application.Abstractions;
using IdentityService.Domain;
using MediatR;
using Salon.BuildingBlocks.Abstractions;
using Salon.BuildingBlocks.Audit;

namespace IdentityService.Application.Users;

public sealed record UserDto(Guid Id, string Email, string FullName, Guid BranchId, string Role, bool IsActive);

public sealed record RegisterUserCommand(string Email, string FullName, Guid BranchId, string Role, string Actor)
    : ICommand<UserDto>, IAuditableRequest
{
    public AuditEvent CreateAuditEvent(object response)
    {
        return new AuditEvent(
            Service: "identity-service",
            Action: "UserRegistered",
            Actor: Actor,
            OccurredOnUtc: DateTime.UtcNow,
            Metadata: new Dictionary<string, object?>
            {
                ["email"] = Email,
                ["branchId"] = BranchId,
                ["role"] = Role
            });
    }
}

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Role).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
    }
}

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(IUserRepository repository, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // This command is for admin-created users, so we need to generate a temporary password
        // In production, you might want to send a password reset email instead
        var temporaryPassword = Guid.NewGuid().ToString("N")[..16]; // Generate a temporary password
        var passwordHash = _passwordHasher.HashPassword(temporaryPassword);
        
        var user = new UserAccount(Guid.NewGuid(), request.Email, request.FullName, request.BranchId, request.Role, passwordHash);
        await _repository.AddAsync(user, cancellationToken);
        return new UserDto(user.Id, user.Email, user.FullName, user.BranchId, user.Role, user.IsActive);
    }
}
