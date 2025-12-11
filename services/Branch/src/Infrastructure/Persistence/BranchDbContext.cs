using BranchService.Application.Abstractions;
using BranchService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Salon.BuildingBlocks.Data;

namespace BranchService.Infrastructure.Persistence;

public sealed class BranchDbContext : ServiceDbContextBase
{
    public BranchDbContext(DbContextOptions<BranchDbContext> options) : base(options)
    {
    }

    public DbSet<Branch> Branches => Set<Branch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Branch>(builder =>
        {
            builder.ToTable("Branches");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Timezone).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Address).IsRequired().HasMaxLength(500);
        });
    }
}

internal sealed class BranchRepository : IBranchRepository
{
    private readonly BranchDbContext _context;

    public BranchRepository(BranchDbContext context)
    {
        _context = context;
    }

    public async Task<Branch?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Branches.FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task AddAsync(Branch branch, CancellationToken cancellationToken)
    {
        await _context.Branches.AddAsync(branch, cancellationToken);
    }
}

public static class BranchInfrastructureRegistration
{
    public static IServiceCollection AddBranchInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<BranchDbContext>(configuration, "BranchDb");
        services.AddScoped<IBranchRepository, BranchRepository>();
        return services;
    }
}
