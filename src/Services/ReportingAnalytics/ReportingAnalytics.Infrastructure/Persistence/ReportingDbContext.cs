using BuildingBlocks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ReportingAnalytics.Application.Abstractions;
using ReportingAnalytics.Domain.Snapshots;

namespace ReportingAnalytics.Infrastructure.Persistence;

public sealed class ReportingDbContext(DbContextOptions<ReportingDbContext> options)
    : AppDbContextBase(options), IReportingDbContext
{
    public DbSet<DailySalesSnapshot> DailySales => Set<DailySalesSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DailySalesSnapshot>(builder =>
        {
            builder.ToTable("DailySales");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.TotalSales).HasPrecision(18, 2);
            builder.Property(x => x.Tax).HasPrecision(18, 2);
            builder.Property(x => x.Discount).HasPrecision(18, 2);
            builder.Property(x => x.BusinessDate)
                .HasConversion(v => v.ToDateTime(TimeOnly.MinValue), v => DateOnly.FromDateTime(v));
        });

        base.OnModelCreating(modelBuilder);
    }
}
