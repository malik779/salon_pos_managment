namespace BookingService.Domain.Entities;

public sealed class Booking
{
    public Booking(Guid id, Guid branchId, Guid clientId, Guid staffId, DateTime startUtc, DateTime endUtc, string status, string idempotencyKey)
    {
        Id = id;
        BranchId = branchId;
        ClientId = clientId;
        StaffId = staffId;
        StartUtc = startUtc;
        EndUtc = endUtc;
        Status = status;
        IdempotencyKey = idempotencyKey;
    }

    public Guid Id { get; }
    public Guid BranchId { get; }
    public Guid ClientId { get; }
    public Guid StaffId { get; }
    public DateTime StartUtc { get; }
    public DateTime EndUtc { get; }
    public string Status { get; private set; }
    public string IdempotencyKey { get; }

    public void UpdateStatus(string status)
    {
        Status = status;
    }
}
