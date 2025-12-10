namespace BookingService.Application.Abstractions;

public interface IAvailabilityService
{
    bool IsAvailable(Guid branchId, Guid staffId, DateTime start, DateTime end);
    void Block(Guid branchId, Guid staffId, DateTime start, DateTime end);
}
