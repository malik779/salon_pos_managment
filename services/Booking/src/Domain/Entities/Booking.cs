namespace BookingService.Domain.Entities;

public sealed class Booking
{
    private Booking()
    {
        Status = string.Empty;
        IdempotencyKey = string.Empty;
    }

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

    public Guid Id { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid StaffId { get; private set; }
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public string Status { get; private set; }
    public string IdempotencyKey { get; private set; }

    public void UpdateStatus(string status)
    {
        Status = status;
    }
}
