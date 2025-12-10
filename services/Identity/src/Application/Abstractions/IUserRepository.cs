using IdentityService.Domain.Entities;

namespace IdentityService.Application.Abstractions;

public interface IUserRepository
{
    Task<User?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
}
