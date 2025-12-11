using BookingService.Application.Abstractions;
using MediatR;
using Salon.BuildingBlocks.Abstractions;

namespace BookingService.Application.Appointments;

public sealed record GetAppointmentByIdQuery(Guid Id) : IQuery<AppointmentDto?>;

public sealed class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, AppointmentDto?>
{
    private readonly IAppointmentRepository _repository;

    public GetAppointmentByIdQueryHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<AppointmentDto?> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _repository.GetAsync(request.Id, cancellationToken);
        return appointment is null
            ? null
            : new AppointmentDto(
                appointment.Id,
                appointment.BranchId,
                appointment.ClientId,
                appointment.StaffId,
                appointment.StartUtc,
                appointment.EndUtc,
                appointment.Status,
                appointment.Source);
    }
}
