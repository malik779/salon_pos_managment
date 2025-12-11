using BookingService.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Persistence;

public sealed class EfAvailabilityService : IAvailabilityService
{
    private readonly BookingDbContext _dbContext;

    public EfAvailabilityService(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsAvailableAsync(Guid branchId, Guid staffId, DateTime start, DateTime end, CancellationToken cancellationToken)
    {
        return !await _dbContext.Bookings.AsNoTracking().AnyAsync(
            b => b.BranchId == branchId && b.StaffId == staffId && TimeOverlaps(b.StartUtc, b.EndUtc, start, end),
            cancellationToken);
    }

    public Task BlockAsync(Guid branchId, Guid staffId, DateTime start, DateTime end, CancellationToken cancellationToken)
    {
        // persistence of bookings enforces availability; nothing to do.
        return Task.CompletedTask;
    }

    private static bool TimeOverlaps(DateTime existingStart, DateTime existingEnd, DateTime newStart, DateTime newEnd)
    {
        return newStart < existingEnd && existingStart < newEnd;
    }
}
