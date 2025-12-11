namespace ReportsService.Domain.Entities;

public sealed class ReportKpi
{
    private ReportKpi()
    {
        Name = string.Empty;
    }

    public ReportKpi(string name, decimal value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; private set; }
    public decimal Value { get; private set; }
}

public sealed class BranchDailyReport
{
    private BranchDailyReport()
    {
    }

    public BranchDailyReport(DateOnly businessDate, decimal sales, int bookings, decimal commissionPayout)
    {
        BusinessDate = businessDate;
        Sales = sales;
        Bookings = bookings;
        CommissionPayout = commissionPayout;
    }

    public DateOnly BusinessDate { get; private set; }
    public decimal Sales { get; private set; }
    public int Bookings { get; private set; }
    public decimal CommissionPayout { get; private set; }
}
