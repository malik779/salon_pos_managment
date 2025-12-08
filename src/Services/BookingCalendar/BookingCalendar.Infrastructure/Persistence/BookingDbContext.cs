using BookingCalendar.Application.Abstractions;
using BookingCalendar.Domain.Appointments;
using BuildingBlocks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendar.Infrastructure.Persistence;

public sealed class BookingDbContext(DbContextOptions<BookingDbContext> options)
    : AppDbContextBase(options), IBookingDbContext
{
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(builder =>
        {
            builder.ToTable("Appointments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.BranchId).IsRequired();
            builder.Property(x => x.ClientId).IsRequired();
            builder.Property(x => x.StaffId).IsRequired();
            builder.Property(x => x.ServiceIds).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.StartUtc).IsRequired();
            builder.Property(x => x.EndUtc).IsRequired();
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
        });

        base.OnModelCreating(modelBuilder);
    }
}
