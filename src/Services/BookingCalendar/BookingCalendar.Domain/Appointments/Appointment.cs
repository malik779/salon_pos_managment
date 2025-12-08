using BuildingBlocks.Domain.Abstractions;

namespace BookingCalendar.Domain.Appointments;

public sealed class Appointment : AuditableEntity, IAggregateRoot
{
    private Appointment()
    {
    }

    private Appointment(
        Guid id,
        Guid branchId,
        Guid clientId,
        Guid staffId,
        string serviceIds,
        DateTime startUtc,
        DateTime endUtc,
        string status)
    {
        Id = id;
        BranchId = branchId;
        ClientId = clientId;
        StaffId = staffId;
        ServiceIds = serviceIds;
        StartUtc = startUtc;
        EndUtc = endUtc;
        Status = status;
    }

    public Guid BranchId { get; private set; }
        = Guid.Empty;
    public Guid ClientId { get; private set; }
        = Guid.Empty;
    public Guid StaffId { get; private set; }
        = Guid.Empty;
    public string ServiceIds { get; private set; } = string.Empty;
    public DateTime StartUtc { get; private set; }
        = DateTime.MinValue;
    public DateTime EndUtc { get; private set; }
        = DateTime.MinValue;
    public string Status { get; private set; } = "Pending";

    public static Appointment Schedule(
        Guid branchId,
        Guid clientId,
        Guid staffId,
        IReadOnlyCollection<Guid> serviceIds,
        DateTime startUtc,
        DateTime endUtc)
        => new(Guid.NewGuid(), branchId, clientId, staffId, string.Join(',', serviceIds), startUtc, endUtc, "Pending");
}
