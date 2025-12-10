using CatalogService.Application.Abstractions;
using CatalogService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ICatalogRepository, InMemoryCatalogRepository>();
        return services;
    }
}
