using IdentityService.Application.Abstractions;
using IdentityService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Salon.BuildingBlocks.Data;

namespace IdentityService.Infrastructure.Persistence;

public sealed class IdentityDbContext : ServiceDbContextBase
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<UserAccount> Users => Set<UserAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>(builder =>
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
            builder.HasIndex(x => x.Email).IsUnique();
            builder.Property(x => x.FullName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Role).IsRequired().HasMaxLength(100);
            builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(500);
            builder.Property(x => x.PasswordResetToken).HasMaxLength(500);
            builder.Property(x => x.RefreshToken).HasMaxLength(500);
        });
    }
}

internal sealed class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _context;

    public UserRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<UserAccount?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Users.FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<UserAccount?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && 
                                      u.RefreshTokenExpiry.HasValue && 
                                      u.RefreshTokenExpiry > DateTime.UtcNow, 
                                 cancellationToken);
    }

    public async Task AddAsync(UserAccount user, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task UpdateAsync(UserAccount user, CancellationToken cancellationToken)
    {
        _context.Users.Update(user);
        await Task.CompletedTask;
    }
}

public static class IdentityInfrastructureRegistration
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<IdentityDbContext>(configuration, "IdentityDb");
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IdentityService.Application.Abstractions.IPasswordHasher, IdentityService.Infrastructure.Services.PasswordHasher>();
        services.AddSingleton<IdentityService.Application.Abstractions.ITokenService, IdentityService.Infrastructure.Services.TokenService>();
        return services;
    }
}
