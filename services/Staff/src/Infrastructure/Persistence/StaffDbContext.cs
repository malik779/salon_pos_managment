using Microsoft.EntityFrameworkCore;
using StaffService.Domain.Entities;

namespace StaffService.Infrastructure.Persistence;

public class StaffDbContext : DbContext
{
    public StaffDbContext(DbContextOptions<StaffDbContext> options) : base(options)
    {
    }

    public DbSet<StaffMember> StaffMembers => Set<StaffMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StaffMember>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FullName).IsRequired();
        });
    }
}
