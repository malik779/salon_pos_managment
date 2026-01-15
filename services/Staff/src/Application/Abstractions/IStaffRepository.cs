using StaffService.Domain;

namespace StaffService.Application.Abstractions;

public interface IStaffRepository
{
    Task<StaffMember?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(StaffMember staffMember, CancellationToken cancellationToken);
    /// <summary>
    /// Gets all staff members without pagination
    /// Optimized for read-only queries using AsNoTracking
    /// </summary>
    Task<List<StaffMember>> GetAllAsync(string? searchTerm, CancellationToken cancellationToken);
}
