using IdentityService.Domain;

namespace IdentityService.Application.Abstractions;

public interface IUserRepository
{
    Task<UserAccount?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(UserAccount user, CancellationToken cancellationToken);
}
