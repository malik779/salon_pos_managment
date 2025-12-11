namespace PosService.Domain;

public class Invoice
{
    private Invoice()
    {
        Status = string.Empty;
    }

    public Invoice(Guid id, Guid branchId, Guid clientId, decimal total)
    {
        Id = id;
        BranchId = branchId;
        ClientId = clientId;
        Total = total;
        Status = "Pending";
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid ClientId { get; private set; }
    public decimal Total { get; private set; }
    public string Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? PaidAtUtc { get; private set; }

    public void MarkPaid()
    {
        Status = "Paid";
        PaidAtUtc = DateTime.UtcNow;
    }
}
