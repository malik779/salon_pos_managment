using BookingService.Domain.Entities;

namespace BookingService.Application.Abstractions;

public interface IBookingRepository
{
    Task AddAsync(Booking booking, CancellationToken cancellationToken);
    Task<Booking?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Booking>> ListAsync(Guid branchId, DateTime from, DateTime to, CancellationToken cancellationToken);
}
