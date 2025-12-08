using BuildingBlocks.Domain.Primitives;

namespace PosTransactions.Domain;

public static class PosErrors
{
    public static readonly Error InvoiceNotFound = new("PosTransactions.InvoiceNotFound", "Invoice not found");
}
