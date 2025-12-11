using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Salon.BuildingBlocks.Extensions;
using StaffService.Application.Abstractions;
using StaffService.Infrastructure.Persistence;

namespace StaffService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddStaffInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<StaffDbContext>(configuration);
        services.AddScoped<IStaffRepository, EfStaffRepository>();
        return services;
    }
}
