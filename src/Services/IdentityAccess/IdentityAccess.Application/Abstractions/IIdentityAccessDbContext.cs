using IdentityAccess.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace IdentityAccess.Application.Abstractions;

public interface IIdentityAccessDbContext
{
    DbSet<User> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
