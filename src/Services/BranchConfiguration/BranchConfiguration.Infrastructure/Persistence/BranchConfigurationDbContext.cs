using BranchConfiguration.Application.Abstractions;
using BranchConfiguration.Domain.Branches;
using BuildingBlocks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BranchConfiguration.Infrastructure.Persistence;

public sealed class BranchConfigurationDbContext(DbContextOptions<BranchConfigurationDbContext> options)
    : AppDbContextBase(options), IBranchConfigurationDbContext
{
    public DbSet<Branch> Branches => Set<Branch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Branch>(builder =>
        {
            builder.ToTable("Branches");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Name).IsRequired().HasMaxLength(200);
            builder.Property(b => b.Timezone).IsRequired().HasMaxLength(100);
            builder.Property(b => b.Address).IsRequired().HasMaxLength(500);
            builder.Property(b => b.DefaultTaxRate).HasPrecision(5, 4);
            builder.Property(b => b.OpenTimeUtc).IsRequired();
            builder.Property(b => b.CloseTimeUtc).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
