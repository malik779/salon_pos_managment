using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Salon.BuildingBlocks.Extensions;
using SyncService.Application.Abstractions;
using SyncService.Infrastructure.Persistence;

namespace SyncService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddSyncInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<SyncDbContext>(configuration);
        services.AddScoped<IDeviceSyncStore, EfDeviceSyncStore>();
        return services;
    }
}
