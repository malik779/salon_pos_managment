namespace SalonSuite.Contracts.Booking;

public sealed record AppointmentDto(
    Guid Id,
    Guid BranchId,
    Guid ClientId,
    Guid StaffId,
    IReadOnlyCollection<Guid> ServiceIds,
    DateTime StartUtc,
    DateTime EndUtc,
    string Status);
