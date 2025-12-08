using BuildingBlocks.Domain.Abstractions;

namespace PosTransactions.Domain.Invoices;

public sealed class Invoice : AuditableEntity, IAggregateRoot
{
    private Invoice()
    {
    }

    private Invoice(Guid id, Guid branchId, Guid clientId, decimal amount, decimal tax, decimal discount, string status)
    {
        Id = id;
        BranchId = branchId;
        ClientId = clientId;
        Amount = amount;
        Tax = tax;
        Discount = discount;
        Status = status;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid BranchId { get; private set; } = Guid.Empty;
    public Guid ClientId { get; private set; } = Guid.Empty;
    public decimal Amount { get; private set; } = 0;
    public decimal Tax { get; private set; } = 0;
    public decimal Discount { get; private set; } = 0;
    public string Status { get; private set; } = "Pending";
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    public static Invoice Create(Guid branchId, Guid clientId, decimal amount, decimal tax, decimal discount)
        => new(Guid.NewGuid(), branchId, clientId, amount, tax, discount, "Pending");
}
