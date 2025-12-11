using BranchService.Application.Abstractions;
using BranchService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BranchService.Infrastructure.Persistence;

public sealed class EfBranchRepository : IBranchRepository
{
    private readonly BranchDbContext _dbContext;

    public EfBranchRepository(BranchDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Branch branch, CancellationToken cancellationToken)
    {
        await _dbContext.Branches.AddAsync(branch, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Branch?> GetAsync(Guid id, CancellationToken cancellationToken)
        => _dbContext.Branches.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Branch>> ListAsync(CancellationToken cancellationToken)
        => await _dbContext.Branches.AsNoTracking().ToListAsync(cancellationToken);
}
