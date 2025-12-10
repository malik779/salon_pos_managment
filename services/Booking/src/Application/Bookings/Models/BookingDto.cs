namespace BookingService.Application.Bookings.Models;

public sealed record BookingDto(Guid Id, Guid BranchId, Guid ClientId, Guid StaffId, DateTime StartUtc, DateTime EndUtc, string Status, string IdempotencyKey);
