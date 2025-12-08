using BuildingBlocks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using PosTransactions.Application.Abstractions;
using PosTransactions.Domain.Invoices;

namespace PosTransactions.Infrastructure.Persistence;

public sealed class PosDbContext(DbContextOptions<PosDbContext> options)
    : AppDbContextBase(options), IPosDbContext
{
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invoice>(builder =>
        {
            builder.ToTable("Invoices");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Amount).HasPrecision(18, 2);
            builder.Property(x => x.Tax).HasPrecision(18, 2);
            builder.Property(x => x.Discount).HasPrecision(18, 2);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
        });

        base.OnModelCreating(modelBuilder);
    }
}
