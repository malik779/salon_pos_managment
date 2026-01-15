using FluentValidation;
using IdentityService.Application.Abstractions;
using MediatR;
using Salon.BuildingBlocks.Abstractions;

namespace IdentityService.Application.Authentication;

public sealed record LoginCommand(string Email, string Password) : ICommand<LoginResponse>;

public sealed record LoginResponse(string AccessToken, string RefreshToken, Guid UserId, string Email, string FullName, string Role, Guid BranchId);

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Role, user.BranchId);
        var refreshToken = _tokenService.GenerateRefreshToken();
        
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        user.SetRefreshToken(refreshToken, refreshTokenExpiry);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return new LoginResponse(
            accessToken,
            refreshToken,
            user.Id,
            user.Email,
            user.FullName,
            user.Role,
            user.BranchId
        );
    }
}
