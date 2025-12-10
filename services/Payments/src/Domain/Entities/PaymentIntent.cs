namespace PaymentsService.Domain.Entities;

public sealed class PaymentIntent
{
    public PaymentIntent(Guid id, Guid invoiceId, decimal amount, string method, string status, string providerReference)
    {
        Id = id;
        InvoiceId = invoiceId;
        Amount = amount;
        Method = method;
        Status = status;
        ProviderReference = providerReference;
    }

    public Guid Id { get; }
    public Guid InvoiceId { get; }
    public decimal Amount { get; }
    public string Method { get; }
    public string Status { get; private set; }
    public string ProviderReference { get; }

    public void MarkCaptured() => Status = "Captured";
}
