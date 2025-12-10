using BookingService.Application.Abstractions;

namespace BookingService.Infrastructure.Persistence;

public sealed class InMemoryAvailabilityService : IAvailabilityService
{
    private readonly HashSet<string> _blocked = new();

    public bool IsAvailable(Guid branchId, Guid staffId, DateTime start, DateTime end)
    {
        return !_blocked.Contains(Token(branchId, staffId, start, end));
    }

    public void Block(Guid branchId, Guid staffId, DateTime start, DateTime end)
    {
        _blocked.Add(Token(branchId, staffId, start, end));
    }

    private static string Token(Guid branchId, Guid staffId, DateTime start, DateTime end) => $"{branchId}:{staffId}:{start:o}:{end:o}";
}
