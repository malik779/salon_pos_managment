using Microsoft.Extensions.DependencyInjection;
using StaffService.Application.Abstractions;
using StaffService.Infrastructure.Persistence;

namespace StaffService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddStaffInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IStaffRepository, InMemoryStaffRepository>();
        return services;
    }
}
