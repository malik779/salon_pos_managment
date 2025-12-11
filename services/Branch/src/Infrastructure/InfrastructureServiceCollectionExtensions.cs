using BranchService.Application.Abstractions;
using BranchService.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Salon.BuildingBlocks.Extensions;

namespace BranchService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddBranchInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<BranchDbContext>(configuration);
        services.AddScoped<IBranchRepository, EfBranchRepository>();
        return services;
    }
}
