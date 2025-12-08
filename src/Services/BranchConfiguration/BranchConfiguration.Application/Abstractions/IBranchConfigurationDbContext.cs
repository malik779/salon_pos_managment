using BranchConfiguration.Domain.Branches;
using Microsoft.EntityFrameworkCore;

namespace BranchConfiguration.Application.Abstractions;

public interface IBranchConfigurationDbContext
{
    DbSet<Branch> Branches { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
