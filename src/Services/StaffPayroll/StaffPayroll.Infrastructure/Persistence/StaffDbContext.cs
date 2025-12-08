using BuildingBlocks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StaffPayroll.Application.Abstractions;
using StaffPayroll.Domain.Staff;

namespace StaffPayroll.Infrastructure.Persistence;

public sealed class StaffDbContext(DbContextOptions<StaffDbContext> options)
    : AppDbContextBase(options), IStaffDbContext
{
    public DbSet<StaffMember> StaffMembers => Set<StaffMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StaffMember>(builder =>
        {
            builder.ToTable("StaffMembers");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.CommissionRate).HasPrecision(5, 4);
            builder.Property(x => x.EmploymentType).IsRequired().HasMaxLength(50);
        });

        base.OnModelCreating(modelBuilder);
    }
}
