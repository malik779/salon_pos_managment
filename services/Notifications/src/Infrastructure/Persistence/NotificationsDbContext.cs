using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationsService.Application.Abstractions;
using NotificationsService.Domain;
using Salon.BuildingBlocks.Data;

namespace NotificationsService.Infrastructure.Persistence;

public sealed class NotificationsDbContext : ServiceDbContextBase
{
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : base(options)
    {
    }

    public DbSet<NotificationMessage> Notifications => Set<NotificationMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationMessage>(builder =>
        {
            builder.ToTable("Notifications");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Channel).HasMaxLength(50).IsRequired();
            builder.Property(x => x.TemplateCode).HasMaxLength(100).IsRequired();
        });
    }
}

internal sealed class NotificationRepository : INotificationRepository
{
    private readonly NotificationsDbContext _context;

    public NotificationRepository(NotificationsDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationMessage?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Notifications.FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task AddAsync(NotificationMessage message, CancellationToken cancellationToken)
    {
        await _context.Notifications.AddAsync(message, cancellationToken);
    }
}

public static class NotificationsInfrastructureRegistration
{
    public static IServiceCollection AddNotificationsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<NotificationsDbContext>(configuration, "NotificationsDb");
        services.AddScoped<INotificationRepository, NotificationRepository>();
        return services;
    }
}
