using BookingService.Domain;

namespace BookingService.Application.Abstractions;

public interface IAppointmentRepository
{
    Task<Appointment?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Appointment appointment, CancellationToken cancellationToken);
}
