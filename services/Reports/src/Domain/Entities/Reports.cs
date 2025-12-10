namespace ReportsService.Domain.Entities;

public sealed record ReportKpi(string Name, decimal Value);
public sealed record BranchDailyReport(DateOnly BusinessDate, decimal Sales, int Bookings, decimal CommissionPayout);
