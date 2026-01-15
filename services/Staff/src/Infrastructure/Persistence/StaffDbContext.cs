using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Salon.BuildingBlocks.Data;
using StaffService.Application.Abstractions;
using StaffService.Domain;

namespace StaffService.Infrastructure.Persistence;

public sealed class StaffDbContext : ServiceDbContextBase
{
    public StaffDbContext(DbContextOptions<StaffDbContext> options) : base(options)
    {
    }

    public DbSet<StaffMember> StaffMembers => Set<StaffMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StaffMember>(builder =>
        {
            builder.ToTable("Staff");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Role).IsRequired().HasMaxLength(100);
        });
    }
}

internal sealed class StaffRepository : IStaffRepository
{
    private readonly StaffDbContext _context;

    public StaffRepository(StaffDbContext context)
    {
        _context = context;
    }

    public async Task<StaffMember?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.StaffMembers.FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task AddAsync(StaffMember staffMember, CancellationToken cancellationToken)
    {
        await _context.StaffMembers.AddAsync(staffMember, cancellationToken);
    }

    public async Task<List<StaffMember>> GetAllAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        var query = _context.StaffMembers.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(s => s.Role.Contains(searchTerm));
        }

        return await query
            .OrderBy(s => s.Role)
            .ToListAsync(cancellationToken);
    }
}

public static class StaffInfrastructureRegistration
{
    public static IServiceCollection AddStaffInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<StaffDbContext>(configuration, "StaffDb");
        services.AddScoped<IStaffRepository, StaffRepository>();
        return services;
    }
}
