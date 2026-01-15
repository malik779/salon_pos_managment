using FluentValidation;
using IdentityService.Application.Abstractions;
using MediatR;
using Salon.BuildingBlocks.Abstractions;

namespace IdentityService.Application.Authentication;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<RefreshTokenResponse>;

public sealed record RefreshTokenResponse(string AccessToken, string RefreshToken);

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);
        
        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Role, user.BranchId);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        user.SetRefreshToken(newRefreshToken, refreshTokenExpiry);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return new RefreshTokenResponse(accessToken, newRefreshToken);
    }
}
