namespace PosService.Domain.Entities;

public sealed class Invoice
{
    public Invoice(Guid id, Guid branchId, Guid clientId, decimal subtotal, decimal tax, decimal discount, decimal total, string status, IReadOnlyList<InvoiceLine> lines)
    {
        Id = id;
        BranchId = branchId;
        ClientId = clientId;
        Subtotal = subtotal;
        Tax = tax;
        Discount = discount;
        Total = total;
        Status = status;
        Lines = lines;
    }

    public Guid Id { get; }
    public Guid BranchId { get; }
    public Guid ClientId { get; }
    public decimal Subtotal { get; }
    public decimal Tax { get; }
    public decimal Discount { get; }
    public decimal Total { get; }
    public string Status { get; private set; }
    public IReadOnlyList<InvoiceLine> Lines { get; }

    public void FinalizeInvoice() => Status = "Finalized";
    public void Refund() => Status = "Refunded";
}

public sealed record InvoiceLine(Guid ItemId, string ItemType, int Quantity, decimal UnitPrice);
