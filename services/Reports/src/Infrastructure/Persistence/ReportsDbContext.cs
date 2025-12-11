using Microsoft.EntityFrameworkCore;
using ReportsService.Domain.Entities;

namespace ReportsService.Infrastructure.Persistence;

public class ReportsDbContext : DbContext
{
    public ReportsDbContext(DbContextOptions<ReportsDbContext> options) : base(options)
    {
    }

    public DbSet<ReportKpi> Kpis => Set<ReportKpi>();
    public DbSet<BranchDailyReport> BranchDailyReports => Set<BranchDailyReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReportKpi>(builder =>
        {
            builder.HasKey(x => x.Name);
        });

        modelBuilder.Entity<BranchDailyReport>(builder =>
        {
            builder.HasKey(x => new { x.BusinessDate, x.Sales, x.Bookings, x.CommissionPayout });
            builder.Property(x => x.BusinessDate)
                .HasConversion(
                    v => v.ToDateTime(TimeOnly.MinValue),
                    v => DateOnly.FromDateTime(v));
        });
    }
}
