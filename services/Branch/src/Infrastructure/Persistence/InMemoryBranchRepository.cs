using BranchService.Application.Abstractions;
using BranchService.Domain.Entities;

namespace BranchService.Infrastructure.Persistence;

public sealed class InMemoryBranchRepository : IBranchRepository
{
    private readonly Dictionary<Guid, Branch> _branches = new();

    public Task AddAsync(Branch branch, CancellationToken cancellationToken)
    {
        _branches[branch.Id] = branch;
        return Task.CompletedTask;
    }

    public Task<Branch?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        _branches.TryGetValue(id, out var branch);
        return Task.FromResult(branch);
    }

    public Task<IReadOnlyList<Branch>> ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Branch> snapshot = _branches.Values.ToList();
        return Task.FromResult(snapshot);
    }
}
