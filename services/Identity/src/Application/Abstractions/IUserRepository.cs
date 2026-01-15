using IdentityService.Domain;

namespace IdentityService.Application.Abstractions;

public interface IUserRepository
{
    Task<UserAccount?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<UserAccount?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task AddAsync(UserAccount user, CancellationToken cancellationToken);
    Task UpdateAsync(UserAccount user, CancellationToken cancellationToken);
}
