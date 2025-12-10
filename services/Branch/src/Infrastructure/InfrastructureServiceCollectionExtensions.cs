using BranchService.Application.Abstractions;
using BranchService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace BranchService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddBranchInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IBranchRepository, InMemoryBranchRepository>();
        return services;
    }
}
