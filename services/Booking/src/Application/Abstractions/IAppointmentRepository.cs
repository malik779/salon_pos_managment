using BookingService.Domain;

namespace BookingService.Application.Abstractions;

public interface IAppointmentRepository
{
    Task<Appointment?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Appointment appointment, CancellationToken cancellationToken);
    /// <summary>
    /// Gets all appointments without pagination
    /// Optimized for read-only queries using AsNoTracking
    /// Note: For large datasets, consider adding date range filtering
    /// </summary>
    Task<List<Appointment>> GetAllAsync(string? searchTerm, CancellationToken cancellationToken);
    Task<List<Appointment>> GetCalendarAsync(Guid branchId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken);
}
