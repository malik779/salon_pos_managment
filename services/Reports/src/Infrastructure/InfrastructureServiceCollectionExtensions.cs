using Microsoft.Extensions.DependencyInjection;
using ReportsService.Application.Abstractions;
using ReportsService.Infrastructure.Persistence;

namespace ReportsService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddReportsInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IReportRepository, InMemoryReportRepository>();
        return services;
    }
}
