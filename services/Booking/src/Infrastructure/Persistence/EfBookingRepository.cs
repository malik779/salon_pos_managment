using BookingService.Application.Abstractions;
using BookingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Persistence;

public sealed class EfBookingRepository : IBookingRepository
{
    private readonly BookingDbContext _dbContext;

    public EfBookingRepository(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken)
    {
        await _dbContext.Bookings.AddAsync(booking, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Booking?> GetAsync(Guid id, CancellationToken cancellationToken)
        => _dbContext.Bookings.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Booking>> ListAsync(Guid branchId, DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        return await _dbContext.Bookings.AsNoTracking()
            .Where(x => x.BranchId == branchId && x.StartUtc >= from && x.EndUtc <= to)
            .OrderBy(x => x.StartUtc)
            .ToListAsync(cancellationToken);
    }
}
