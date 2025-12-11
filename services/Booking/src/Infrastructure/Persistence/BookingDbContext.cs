using BookingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Persistence;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
    }

    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Status).IsRequired();
        });

        modelBuilder.Entity<IdempotencyRecord>(builder =>
        {
            builder.HasKey(x => x.Key);
            builder.Property(x => x.BookingId).IsRequired();
        });
    }
}

public class IdempotencyRecord
{
    public string Key { get; set; } = string.Empty;
    public Guid BookingId { get; set; }
}
