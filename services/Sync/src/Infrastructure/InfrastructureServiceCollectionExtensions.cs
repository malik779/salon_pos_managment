using Microsoft.Extensions.DependencyInjection;
using SyncService.Application.Abstractions;
using SyncService.Infrastructure.Persistence;

namespace SyncService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddSyncInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDeviceSyncStore, InMemoryDeviceSyncStore>();
        return services;
    }
}
