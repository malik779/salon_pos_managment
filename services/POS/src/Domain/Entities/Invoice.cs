namespace PosService.Domain.Entities;

public sealed class Invoice
{
    private Invoice()
    {
        Lines = new List<InvoiceLine>();
        Status = "Draft";
    }

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
        Lines = lines.ToList();
    }

    public Guid Id { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid ClientId { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal Tax { get; private set; }
    public decimal Discount { get; private set; }
    public decimal Total { get; private set; }
    public string Status { get; private set; }
    public List<InvoiceLine> Lines { get; private set; }

    public void FinalizeInvoice() => Status = "Finalized";
    public void Refund() => Status = "Refunded";
}

public sealed class InvoiceLine
{
    public InvoiceLine(Guid itemId, string itemType, int quantity, decimal unitPrice)
    {
        ItemId = itemId;
        ItemType = itemType;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    private InvoiceLine()
    {
    }

    public Guid ItemId { get; private set; }
    public string ItemType { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
}
