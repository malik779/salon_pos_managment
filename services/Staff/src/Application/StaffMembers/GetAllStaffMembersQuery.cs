using MediatR;
using Salon.BuildingBlocks.Abstractions;
using StaffService.Application.Abstractions;

namespace StaffService.Application.StaffMembers;

/// <summary>
/// Query to fetch all staff members without pagination
/// Optimized for dropdowns, selects, and other UI components that need complete lists
/// Performance: Uses AsNoTracking for read-only queries to reduce memory overhead
/// </summary>
public sealed record GetAllStaffMembersQuery(string? SearchTerm = null) : IQuery<List<StaffMemberDto>>;

public sealed class GetAllStaffMembersQueryHandler : IRequestHandler<GetAllStaffMembersQuery, List<StaffMemberDto>>
{
    private readonly IStaffRepository _repository;

    public GetAllStaffMembersQueryHandler(IStaffRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<StaffMemberDto>> Handle(GetAllStaffMembersQuery request, CancellationToken cancellationToken)
    {
        var staffMembers = await _repository.GetAllAsync(request.SearchTerm, cancellationToken);
        return staffMembers.Select(s => new StaffMemberDto(s.Id, s.UserId, s.DefaultBranchId, s.Role, s.IsActive)).ToList();
    }
}
