namespace PosService.Application.Invoices.Models;

public sealed record InvoiceDto(Guid Id, Guid BranchId, Guid ClientId, decimal Subtotal, decimal Tax, decimal Discount, decimal Total, string Status);
public sealed record PaymentDto(Guid PaymentId, Guid InvoiceId, string Method, decimal Amount, string Status);
public sealed record CloseDaySummary(Guid BranchId, DateOnly BusinessDate, decimal TotalSales, decimal CashTotal, decimal CardTotal, decimal Refunds);
