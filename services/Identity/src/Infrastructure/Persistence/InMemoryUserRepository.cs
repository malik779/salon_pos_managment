using IdentityService.Application.Abstractions;
using IdentityService.Domain.Entities;

namespace IdentityService.Infrastructure.Persistence;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new();

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _users.Add(user);
        return Task.CompletedTask;
    }

    public Task<User?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_users.FirstOrDefault(x => x.Id == id));
    }

    public Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<User> snapshot = _users.ToList();
        return Task.FromResult(snapshot);
    }
}
