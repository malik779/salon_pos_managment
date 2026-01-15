using FluentValidation;
using IdentityService.Application.Abstractions;
using MediatR;
using Salon.BuildingBlocks.Abstractions;

namespace IdentityService.Application.Authentication;

public sealed record ResetPasswordCommand(string Email, string ResetToken, string NewPassword) 
    : ICommand<ResetPasswordResponse>;

public sealed record ResetPasswordResponse(bool Success);

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.ResetToken).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number");
    }
}

public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<ResetPasswordResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        if (user is null || 
            user.PasswordResetToken != request.ResetToken ||
            user.PasswordResetTokenExpiry is null ||
            user.PasswordResetTokenExpiry < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired reset token");
        }

        var passwordHash = _passwordHasher.HashPassword(request.NewPassword);
        user.UpdatePassword(passwordHash);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return new ResetPasswordResponse(true);
    }
}
