using IdentityService.Application.Abstractions;
using IdentityService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<IDeviceRegistry, InMemoryDeviceRegistry>();
        return services;
    }
}
