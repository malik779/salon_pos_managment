using FluentValidation;
using MediatR;

namespace IdentityService.Application.Auth.Commands.GenerateToken;

public sealed record GenerateTokenCommand(string Username, string Password) : IRequest<TokenResponse>;

public sealed record TokenResponse(string AccessToken, string RefreshToken, int ExpiresIn);

public sealed class GenerateTokenCommandValidator : AbstractValidator<GenerateTokenCommand>
{
    public GenerateTokenCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public sealed class GenerateTokenCommandHandler : IRequestHandler<GenerateTokenCommand, TokenResponse>
{
    public Task<TokenResponse> Handle(GenerateTokenCommand request, CancellationToken cancellationToken)
    {
        var response = new TokenResponse(
            Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
            Guid.NewGuid().ToString("N"),
            3600
        );
        return Task.FromResult(response);
    }
}
