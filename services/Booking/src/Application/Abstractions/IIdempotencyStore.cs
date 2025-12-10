using BookingService.Domain.Entities;

namespace BookingService.Application.Abstractions;

public interface IIdempotencyStore
{
    Task<Booking?> GetAsync(string key, CancellationToken cancellationToken);
    Task RememberAsync(string key, Booking booking, CancellationToken cancellationToken);
}
