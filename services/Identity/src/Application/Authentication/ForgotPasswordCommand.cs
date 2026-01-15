using FluentValidation;
using IdentityService.Application.Abstractions;
using MediatR;
using Salon.BuildingBlocks.Abstractions;
using System.Security.Cryptography;

namespace IdentityService.Application.Authentication;

public sealed record ForgotPasswordCommand(string Email) : ICommand<ForgotPasswordResponse>;

public sealed record ForgotPasswordResponse(bool Success, string? ResetToken = null);

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    private readonly IUserRepository _userRepository;

    public ForgotPasswordCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        // Don't reveal if user exists or not for security
        if (user is null || !user.IsActive)
        {
            return new ForgotPasswordResponse(true);
        }

        // Generate reset token
        var resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var expiry = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour
        
        user.SetPasswordResetToken(resetToken, expiry);
        await _userRepository.UpdateAsync(user, cancellationToken);

        // In a real application, you would send an email here with the reset token
        // For now, we'll return it (in production, don't return the token)
        return new ForgotPasswordResponse(true, resetToken);
    }
}
