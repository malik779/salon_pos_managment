namespace PaymentsService.Application.Payments.Models;

public sealed record PaymentIntentDto(Guid Id, Guid InvoiceId, decimal Amount, string Status, string ProviderReference);
public sealed record TokenizedCard(string Token, string Brand, string Last4);
