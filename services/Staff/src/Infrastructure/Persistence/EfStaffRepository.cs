using Microsoft.EntityFrameworkCore;
using StaffService.Application.Abstractions;
using StaffService.Domain.Entities;

namespace StaffService.Infrastructure.Persistence;

public sealed class EfStaffRepository : IStaffRepository
{
    private readonly StaffDbContext _dbContext;

    public EfStaffRepository(StaffDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(StaffMember staff, CancellationToken cancellationToken)
    {
        await _dbContext.StaffMembers.AddAsync(staff, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StaffMember>> ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.StaffMembers.AsNoTracking().ToListAsync(cancellationToken);
    }
}
