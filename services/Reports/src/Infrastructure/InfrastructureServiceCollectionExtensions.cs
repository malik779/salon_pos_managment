using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReportsService.Application.Abstractions;
using ReportsService.Infrastructure.Persistence;
using Salon.BuildingBlocks.Extensions;

namespace ReportsService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddReportsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<ReportsDbContext>(configuration);
        services.AddScoped<IReportRepository, EfReportRepository>();
        return services;
    }
}
