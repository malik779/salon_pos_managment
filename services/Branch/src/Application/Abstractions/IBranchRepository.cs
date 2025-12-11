using BranchService.Domain;

namespace BranchService.Application.Abstractions;

public interface IBranchRepository
{
    Task<Branch?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Branch branch, CancellationToken cancellationToken);
}
