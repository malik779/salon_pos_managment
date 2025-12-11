using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReportsService.Application.Abstractions;
using ReportsService.Domain;
using Salon.BuildingBlocks.Data;

namespace ReportsService.Infrastructure.Persistence;

public sealed class ReportsDbContext : ServiceDbContextBase
{
    public ReportsDbContext(DbContextOptions<ReportsDbContext> options) : base(options)
    {
    }

    public DbSet<ReportSnapshot> ReportSnapshots => Set<ReportSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReportSnapshot>(builder =>
        {
            builder.ToTable("ReportSnapshots");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.BranchId, x.Day }).IsUnique();
            builder.Property(x => x.Day).HasConversion(v => v.ToDateTime(TimeOnly.MinValue), v => DateOnly.FromDateTime(v));
            builder.Property(x => x.TotalRevenue).HasColumnType("decimal(18,2)");
        });
    }
}

internal sealed class ReportRepository : IReportRepository
{
    private readonly ReportsDbContext _context;

    public ReportRepository(ReportsDbContext context)
    {
        _context = context;
    }

    public async Task<ReportSnapshot?> GetAsync(Guid branchId, DateOnly day, CancellationToken cancellationToken)
    {
        return await _context.ReportSnapshots.FirstOrDefaultAsync(x => x.BranchId == branchId && x.Day == day, cancellationToken);
    }

    public async Task UpsertAsync(ReportSnapshot snapshot, CancellationToken cancellationToken)
    {
        var existing = await _context.ReportSnapshots.FirstOrDefaultAsync(x => x.BranchId == snapshot.BranchId && x.Day == snapshot.Day, cancellationToken);
        if (existing is null)
        {
            await _context.ReportSnapshots.AddAsync(snapshot, cancellationToken);
        }
        else
        {
            existing.Update(snapshot.TotalRevenue);
        }
    }
}

public static class ReportsInfrastructureRegistration
{
    public static IServiceCollection AddReportsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<ReportsDbContext>(configuration, "ReportsDb");
        services.AddScoped<IReportRepository, ReportRepository>();
        return services;
    }
}
