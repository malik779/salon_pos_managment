using Microsoft.EntityFrameworkCore;
using PosService.Domain.Entities;

namespace PosService.Infrastructure.Persistence;

public class PosDbContext : DbContext
{
    public PosDbContext(DbContextOptions<PosDbContext> options) : base(options)
    {
    }

    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invoice>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Status).IsRequired();
            builder.OwnsMany(x => x.Lines, navigationBuilder =>
            {
                navigationBuilder.WithOwner().HasForeignKey("InvoiceId");
                navigationBuilder.Property<Guid>("Id");
                navigationBuilder.HasKey("Id");
            });
        });
    }
}
