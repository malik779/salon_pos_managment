using BuildingBlocks.Infrastructure.Data;
using IdentityAccess.Application.Abstractions;
using IdentityAccess.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace IdentityAccess.Infrastructure.Persistence;

public sealed class IdentityAccessDbContext(DbContextOptions<IdentityAccessDbContext> options)
    : AppDbContextBase(options), IIdentityAccessDbContext
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.DefaultBranchId).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
