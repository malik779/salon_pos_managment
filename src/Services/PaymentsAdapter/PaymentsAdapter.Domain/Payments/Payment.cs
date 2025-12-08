using BuildingBlocks.Domain.Abstractions;

namespace PaymentsAdapter.Domain.Payments;

public sealed class Payment : AuditableEntity, IAggregateRoot
{
    private Payment()
    {
    }

    private Payment(Guid id, Guid invoiceId, decimal amount, string method)
    {
        Id = id;
        InvoiceId = invoiceId;
        Amount = amount;
        Method = method;
        Status = "Captured";
        CapturedAtUtc = DateTime.UtcNow;
    }

    public Guid InvoiceId { get; private set; } = Guid.Empty;
    public decimal Amount { get; private set; } = 0;
    public string Method { get; private set; } = default!;
    public string Status { get; private set; } = "Captured";
    public DateTime? CapturedAtUtc { get; private set; }
        = null;

    public static Payment Capture(Guid invoiceId, decimal amount, string method)
        => new(Guid.NewGuid(), invoiceId, amount, method);
}
