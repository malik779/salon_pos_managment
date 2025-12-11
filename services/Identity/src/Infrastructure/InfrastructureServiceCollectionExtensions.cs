using IdentityService.Application.Abstractions;
using IdentityService.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Salon.BuildingBlocks.Extensions;

namespace IdentityService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<IdentityDbContext>(configuration);
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IDeviceRegistry, EfDeviceRegistry>();
        return services;
    }
}
