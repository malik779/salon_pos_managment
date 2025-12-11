namespace BookingService.Domain;

public class Appointment
{
    private Appointment()
    {
        Status = string.Empty;
        Source = string.Empty;
    }

    public Appointment(Guid id, Guid branchId, Guid clientId, Guid staffId, DateTime startUtc, TimeSpan duration, string source)
    {
        Id = id;
        BranchId = branchId;
        ClientId = clientId;
        StaffId = staffId;
        StartUtc = startUtc;
        EndUtc = startUtc + duration;
        Source = source;
        Status = "Confirmed";
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid StaffId { get; private set; }
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public string Status { get; private set; }
    public string Source { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
}
