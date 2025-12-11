namespace PaymentsService.Domain.Entities;

public sealed class PaymentIntent
{
    private PaymentIntent()
    {
        Status = string.Empty;
        Method = string.Empty;
        ProviderReference = string.Empty;
    }

    public PaymentIntent(Guid id, Guid invoiceId, decimal amount, string method, string status, string providerReference)
    {
        Id = id;
        InvoiceId = invoiceId;
        Amount = amount;
        Method = method;
        Status = status;
        ProviderReference = providerReference;
    }

    public Guid Id { get; private set; }
    public Guid InvoiceId { get; private set; }
    public decimal Amount { get; private set; }
    public string Method { get; private set; }
    public string Status { get; private set; }
    public string ProviderReference { get; private set; }

    public void MarkCaptured() => Status = "Captured";
}
