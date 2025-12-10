using StaffService.Domain.Entities;

namespace StaffService.Application.Abstractions;

public interface IStaffRepository
{
    Task AddAsync(StaffMember staff, CancellationToken cancellationToken);
    Task<IReadOnlyList<StaffMember>> ListAsync(CancellationToken cancellationToken);
}
