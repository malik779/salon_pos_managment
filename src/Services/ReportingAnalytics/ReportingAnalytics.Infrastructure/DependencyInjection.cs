using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReportingAnalytics.Application.Abstractions;
using ReportingAnalytics.Infrastructure.Persistence;

namespace ReportingAnalytics.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddReportingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ReportingAnalytics")
                               ?? configuration.GetConnectionString("Default")
                               ?? "Server=sqlserver;Database=ReportingAnalytics;User=sa;Password=Your_password123;TrustServerCertificate=True";

        services.AddDbContext<ReportingDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IReportingDbContext>(sp => sp.GetRequiredService<ReportingDbContext>());
        services.AddScoped<IUnitOfWork, EfUnitOfWork<ReportingDbContext>>();

        return services;
    }
}
