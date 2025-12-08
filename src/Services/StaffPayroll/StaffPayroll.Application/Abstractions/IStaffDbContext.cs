using Microsoft.EntityFrameworkCore;
using StaffPayroll.Domain.Staff;

namespace StaffPayroll.Application.Abstractions;

public interface IStaffDbContext
{
    DbSet<StaffMember> StaffMembers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
