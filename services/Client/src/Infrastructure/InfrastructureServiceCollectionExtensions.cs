using ClientService.Application.Abstractions;
using ClientService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ClientService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddClientInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IClientRepository, InMemoryClientRepository>();
        return services;
    }
}
