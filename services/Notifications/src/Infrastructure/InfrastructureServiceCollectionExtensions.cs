using Microsoft.Extensions.DependencyInjection;
using NotificationsService.Application.Abstractions;
using NotificationsService.Infrastructure.Persistence;

namespace NotificationsService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationsInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<INotificationRepository, InMemoryNotificationRepository>();
        return services;
    }
}
