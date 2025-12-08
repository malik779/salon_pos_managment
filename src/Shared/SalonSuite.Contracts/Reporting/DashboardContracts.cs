namespace SalonSuite.Contracts.Reporting;

public sealed record DailySalesKpi(decimal TotalSales, decimal TaxCollected, decimal Discounts, int Invoices);

public sealed record BookingOccupancyKpi(int TotalSlots, int BookedSlots, int CancelledSlots);
