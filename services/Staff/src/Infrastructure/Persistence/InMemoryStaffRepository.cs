using StaffService.Application.Abstractions;
using StaffService.Domain.Entities;

namespace StaffService.Infrastructure.Persistence;

public sealed class InMemoryStaffRepository : IStaffRepository
{
    private readonly List<StaffMember> _staff = new();

    public Task AddAsync(StaffMember staff, CancellationToken cancellationToken)
    {
        _staff.Add(staff);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<StaffMember>> ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<StaffMember> snapshot = _staff.ToList();
        return Task.FromResult(snapshot);
    }
}
