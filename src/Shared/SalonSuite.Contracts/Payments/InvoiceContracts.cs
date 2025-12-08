namespace SalonSuite.Contracts.Payments;

public sealed record InvoiceDto(
    Guid Id,
    Guid BranchId,
    Guid ClientId,
    decimal Amount,
    decimal Tax,
    decimal Discount,
    string PaymentStatus,
    DateTime CreatedAtUtc);
