using BranchService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BranchService.Infrastructure.Persistence;

public class BranchDbContext : DbContext
{
    public BranchDbContext(DbContextOptions<BranchDbContext> options) : base(options)
    {
    }

    public DbSet<Branch> Branches => Set<Branch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Branch>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Timezone).IsRequired();
        });
    }
}
