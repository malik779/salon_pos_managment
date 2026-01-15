using FluentValidation;
using IdentityService.Application.Abstractions;
using IdentityService.Domain;
using MediatR;
using Salon.BuildingBlocks.Abstractions;

namespace IdentityService.Application.Authentication;

public sealed record RegisterCommand(string Email, string Password, string FullName, Guid BranchId, string Role) 
    : ICommand<RegisterResponse>;

public sealed record RegisterResponse(Guid UserId, string Email, string FullName, string Role, Guid BranchId);

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number");
        RuleFor(x => x.FullName).NotEmpty().MinimumLength(2);
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.Role).NotEmpty();
    }
}

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = new UserAccount(
            Guid.NewGuid(),
            request.Email,
            request.FullName,
            request.BranchId,
            request.Role,
            passwordHash
        );

        await _userRepository.AddAsync(user, cancellationToken);

        return new RegisterResponse(
            user.Id,
            user.Email,
            user.FullName,
            user.Role,
            user.BranchId
        );
    }
}
