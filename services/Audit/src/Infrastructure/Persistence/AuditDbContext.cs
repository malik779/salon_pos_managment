using AuditService.Application.Abstractions;
using AuditService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Salon.BuildingBlocks.Data;

namespace AuditService.Infrastructure.Persistence;

public sealed class AuditDbContext : ServiceDbContextBase
{
    public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options)
    {
    }

    public DbSet<AuditRecord> AuditRecords => Set<AuditRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditRecord>(builder =>
        {
            builder.ToTable("AuditRecords");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Service).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Action).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Actor).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Payload).HasColumnType("nvarchar(max)");
        });
    }
}

internal sealed class AuditRepository : IAuditRepository
{
    private readonly AuditDbContext _context;

    public AuditRepository(AuditDbContext context)
    {
        _context = context;
    }

    public async Task<AuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.AuditRecords.FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task AddAsync(AuditRecord record, CancellationToken cancellationToken)
    {
        await _context.AuditRecords.AddAsync(record, cancellationToken);
    }
}

public static class AuditInfrastructureRegistration
{
    public static IServiceCollection AddAuditInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<AuditDbContext>(configuration, "AuditDb");
        services.AddScoped<IAuditRepository, AuditRepository>();
        return services;
    }
}
