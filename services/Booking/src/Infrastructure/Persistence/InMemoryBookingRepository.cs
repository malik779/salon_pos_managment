using BookingService.Application.Abstractions;
using BookingService.Domain.Entities;

namespace BookingService.Infrastructure.Persistence;

public sealed class InMemoryBookingRepository : IBookingRepository
{
    private readonly Dictionary<Guid, Booking> _bookings = new();

    public Task AddAsync(Booking booking, CancellationToken cancellationToken)
    {
        _bookings[booking.Id] = booking;
        return Task.CompletedTask;
    }

    public Task<Booking?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        _bookings.TryGetValue(id, out var booking);
        return Task.FromResult(booking);
    }

    public Task<IReadOnlyList<Booking>> ListAsync(Guid branchId, DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        IReadOnlyList<Booking> results = _bookings.Values
            .Where(x => x.BranchId == branchId && x.StartUtc >= from && x.EndUtc <= to)
            .OrderBy(x => x.StartUtc)
            .ToList();
        return Task.FromResult(results);
    }
}
