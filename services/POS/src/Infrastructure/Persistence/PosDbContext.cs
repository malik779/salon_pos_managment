using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PosService.Application.Abstractions;
using PosService.Domain;
using Salon.BuildingBlocks.Data;

namespace PosService.Infrastructure.Persistence;

public sealed class PosDbContext : ServiceDbContextBase
{
    public PosDbContext(DbContextOptions<PosDbContext> options) : base(options)
    {
    }

    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invoice>(builder =>
        {
            builder.ToTable("Invoices");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Total).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
        });
    }
}

internal sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly PosDbContext _context;

    public InvoiceRepository(PosDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Invoices.FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        await _context.Invoices.AddAsync(invoice, cancellationToken);
    }

    public async Task<List<Invoice>> GetAllAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        var query = _context.Invoices.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(i => i.Status.Contains(searchTerm));
        }

        return await query
            .OrderByDescending(i => i.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}

public static class PosInfrastructureRegistration
{
    public static IServiceCollection AddPosInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<PosDbContext>(configuration, "PosDb");
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        return services;
    }
}
