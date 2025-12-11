namespace ReportsService.Domain;

public class ReportSnapshot
{
    private ReportSnapshot()
    {
    }

    public ReportSnapshot(Guid id, Guid branchId, DateOnly day, decimal totalRevenue)
    {
        Id = id;
        BranchId = branchId;
        Day = day;
        TotalRevenue = totalRevenue;
        GeneratedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid BranchId { get; private set; }
    public DateOnly Day { get; private set; }
    public decimal TotalRevenue { get; private set; }
    public DateTime GeneratedAtUtc { get; private set; }

    public void Update(decimal totalRevenue)
    {
        TotalRevenue = totalRevenue;
        GeneratedAtUtc = DateTime.UtcNow;
    }
}
