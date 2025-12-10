using MediatR;
using StaffService.Application.Abstractions;
using StaffService.Application.Staff.Models;

namespace StaffService.Application.Staff.Queries.ListStaff;

public sealed record ListStaffQuery() : IRequest<IReadOnlyList<StaffDto>>;

public sealed class ListStaffQueryHandler : IRequestHandler<ListStaffQuery, IReadOnlyList<StaffDto>>
{
    private readonly IStaffRepository _repository;

    public ListStaffQueryHandler(IStaffRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<StaffDto>> Handle(ListStaffQuery request, CancellationToken cancellationToken)
    {
        var staff = await _repository.ListAsync(cancellationToken);
        return staff.Select(s => new StaffDto(s.Id, s.FullName, s.DefaultBranchId, s.Role)).ToList();
    }
}
