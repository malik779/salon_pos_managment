using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationsService.Application.Abstractions;
using NotificationsService.Infrastructure.Persistence;
using Salon.BuildingBlocks.Extensions;

namespace NotificationsService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<NotificationsDbContext>(configuration);
        services.AddScoped<INotificationRepository, EfNotificationRepository>();
        return services;
    }
}
