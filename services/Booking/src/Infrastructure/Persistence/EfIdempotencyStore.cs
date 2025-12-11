using BookingService.Application.Abstractions;
using BookingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Persistence;

public sealed class EfIdempotencyStore : IIdempotencyStore
{
    private readonly BookingDbContext _dbContext;

    public EfIdempotencyStore(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Booking?> GetAsync(string key, CancellationToken cancellationToken)
    {
        var record = await _dbContext.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(x => x.Key == key, cancellationToken);
        if (record is null)
        {
            return null;
        }

        return await _dbContext.Bookings.FirstOrDefaultAsync(x => x.Id == record.BookingId, cancellationToken);
    }

    public async Task RememberAsync(string key, Booking booking, CancellationToken cancellationToken)
    {
        _dbContext.IdempotencyRecords.Add(new IdempotencyRecord { Key = key, BookingId = booking.Id });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
