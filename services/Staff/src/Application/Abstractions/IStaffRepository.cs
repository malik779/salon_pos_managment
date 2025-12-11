using StaffService.Domain;

namespace StaffService.Application.Abstractions;

public interface IStaffRepository
{
    Task<StaffMember?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(StaffMember staffMember, CancellationToken cancellationToken);
}
