using BuildingBlocks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using PaymentsAdapter.Application.Abstractions;
using PaymentsAdapter.Domain.Payments;

namespace PaymentsAdapter.Infrastructure.Persistence;

public sealed class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
    : AppDbContextBase(options), IPaymentsDbContext
{
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(builder =>
        {
            builder.ToTable("Payments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Method).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Amount).HasPrecision(18, 2);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
        });

        base.OnModelCreating(modelBuilder);
    }
}
