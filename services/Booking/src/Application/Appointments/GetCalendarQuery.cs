using BookingService.Application.Abstractions;
using MediatR;
using Salon.BuildingBlocks.Abstractions;

namespace BookingService.Application.Appointments;

public sealed record GetCalendarQuery(Guid BranchId, DateTime FromUtc, DateTime ToUtc) : IQuery<List<AppointmentDto>>;

public sealed class GetCalendarQueryHandler : IRequestHandler<GetCalendarQuery, List<AppointmentDto>>
{
    private readonly IAppointmentRepository _repository;

    public GetCalendarQueryHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AppointmentDto>> Handle(GetCalendarQuery request, CancellationToken cancellationToken)
    {
        var appointments = await _repository.GetCalendarAsync(request.BranchId, request.FromUtc, request.ToUtc, cancellationToken);
        return appointments.Select(a => new AppointmentDto(
            a.Id,
            a.BranchId,
            a.ClientId,
            a.StaffId,
            a.StartUtc,
            a.EndUtc,
            a.Status,
            a.Source)).ToList();
    }
}
