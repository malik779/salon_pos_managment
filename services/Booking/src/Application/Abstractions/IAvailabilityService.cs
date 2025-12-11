namespace BookingService.Application.Abstractions;

public interface IAvailabilityService
{
    Task<bool> IsAvailableAsync(Guid branchId, Guid staffId, DateTime start, DateTime end, CancellationToken cancellationToken);
    Task BlockAsync(Guid branchId, Guid staffId, DateTime start, DateTime end, CancellationToken cancellationToken);
}
