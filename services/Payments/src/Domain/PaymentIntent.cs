namespace PaymentsService.Domain;

public class PaymentIntent
{
    private PaymentIntent()
    {
        Provider = string.Empty;
        Status = string.Empty;
    }

    public PaymentIntent(Guid id, Guid invoiceId, decimal amount, string provider)
    {
        Id = id;
        InvoiceId = invoiceId;
        Amount = amount;
        Provider = provider;
        Status = "Pending";
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid InvoiceId { get; private set; }
    public decimal Amount { get; private set; }
    public string Provider { get; private set; }
    public string Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public void MarkAuthorized()
    {
        Status = "Authorized";
    }
}
