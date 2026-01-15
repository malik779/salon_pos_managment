using System.Security.Claims;

namespace IdentityService.Application.Abstractions;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email, string role, Guid branchId);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
}
