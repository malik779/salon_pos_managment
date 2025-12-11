using ClientService.Application.Abstractions;
using ClientService.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Salon.BuildingBlocks.Extensions;

namespace ClientService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddClientInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<ClientDbContext>(configuration);
        services.AddScoped<IClientRepository, EfClientRepository>();
        return services;
    }
}
