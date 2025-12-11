using MediatR;
using Salon.BuildingBlocks.Abstractions;
using StaffService.Application.Abstractions;

namespace StaffService.Application.StaffMembers;

public sealed record GetStaffMemberByIdQuery(Guid Id) : IQuery<StaffMemberDto?>;

public sealed class GetStaffMemberByIdQueryHandler : IRequestHandler<GetStaffMemberByIdQuery, StaffMemberDto?>
{
    private readonly IStaffRepository _repository;

    public GetStaffMemberByIdQueryHandler(IStaffRepository repository)
    {
        _repository = repository;
    }

    public async Task<StaffMemberDto?> Handle(GetStaffMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var staff = await _repository.GetAsync(request.Id, cancellationToken);
        return staff is null
            ? null
            : new StaffMemberDto(staff.Id, staff.UserId, staff.DefaultBranchId, staff.Role, staff.IsActive);
    }
}
