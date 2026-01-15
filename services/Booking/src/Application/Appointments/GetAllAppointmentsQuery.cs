using BookingService.Application.Abstractions;
using MediatR;
using Salon.BuildingBlocks.Abstractions;

namespace BookingService.Application.Appointments;

/// <summary>
/// Query to fetch all appointments without pagination
/// Optimized for dropdowns, selects, and other UI components that need complete lists
/// Performance: Uses AsNoTracking for read-only queries to reduce memory overhead
/// Note: Consider filtering by date range for better performance with large datasets
/// </summary>
public sealed record GetAllAppointmentsQuery(string? SearchTerm = null) : IQuery<List<AppointmentDto>>;

public sealed class GetAllAppointmentsQueryHandler : IRequestHandler<GetAllAppointmentsQuery, List<AppointmentDto>>
{
    private readonly IAppointmentRepository _repository;

    public GetAllAppointmentsQueryHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AppointmentDto>> Handle(GetAllAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var appointments = await _repository.GetAllAsync(request.SearchTerm, cancellationToken);
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
