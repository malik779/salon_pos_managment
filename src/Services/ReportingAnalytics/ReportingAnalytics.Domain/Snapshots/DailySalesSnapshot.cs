using BuildingBlocks.Domain.Abstractions;

namespace ReportingAnalytics.Domain.Snapshots;

public sealed class DailySalesSnapshot : AuditableEntity, IAggregateRoot
{
    private DailySalesSnapshot()
    {
    }

    private DailySalesSnapshot(Guid id, Guid branchId, DateOnly businessDate, decimal totalSales, decimal tax, decimal discount, int invoices)
    {
        Id = id;
        BranchId = branchId;
        BusinessDate = businessDate;
        TotalSales = totalSales;
        Tax = tax;
        Discount = discount;
        Invoices = invoices;
    }

    public Guid BranchId { get; private set; } = Guid.Empty;
    public DateOnly BusinessDate { get; private set; }
        = DateOnly.FromDateTime(DateTime.UtcNow);
    public decimal TotalSales { get; private set; }
        = 0;
    public decimal Tax { get; private set; }
        = 0;
    public decimal Discount { get; private set; }
        = 0;
    public int Invoices { get; private set; }
        = 0;

    public static DailySalesSnapshot Record(Guid branchId, DateOnly businessDate, decimal totalSales, decimal tax, decimal discount, int invoices)
        => new(Guid.NewGuid(), branchId, businessDate, totalSales, tax, discount, invoices);
}
