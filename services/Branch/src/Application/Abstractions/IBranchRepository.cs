using BranchService.Domain.Entities;

namespace BranchService.Application.Abstractions;

public interface IBranchRepository
{
    Task<Branch?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Branch>> ListAsync(CancellationToken cancellationToken);
    Task AddAsync(Branch branch, CancellationToken cancellationToken);
}
