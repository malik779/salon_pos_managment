using BranchService.Domain;

namespace BranchService.Application.Abstractions;

public interface IBranchRepository
{
    Task<Branch?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Branch branch, CancellationToken cancellationToken);
    Task UpdateAsync(Branch branch, CancellationToken cancellationToken);
    Task DeleteAsync(Branch branch, CancellationToken cancellationToken);
    Task<(List<Branch> Items, int TotalCount)> GetBranchesAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm,
        CancellationToken cancellationToken);
    /// <summary>
    /// Gets all branches without pagination
    /// Optimized for read-only queries using AsNoTracking
    /// </summary>
    Task<List<Branch>> GetAllAsync(string? searchTerm, CancellationToken cancellationToken);
}
