using BookingService.Application.Abstractions;
using BookingService.Domain.Entities;

namespace BookingService.Infrastructure.Persistence;

public sealed class InMemoryIdempotencyStore : IIdempotencyStore
{
    private readonly Dictionary<string, Booking> _cache = new();

    public Task<Booking?> GetAsync(string key, CancellationToken cancellationToken)
    {
        _cache.TryGetValue(key, out var booking);
        return Task.FromResult(booking);
    }

    public Task RememberAsync(string key, Booking booking, CancellationToken cancellationToken)
    {
        _cache[key] = booking;
        return Task.CompletedTask;
    }
}
