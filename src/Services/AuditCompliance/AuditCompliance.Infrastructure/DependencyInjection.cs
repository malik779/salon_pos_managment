using AuditCompliance.Application.Abstractions;
using AuditCompliance.Infrastructure.Persistence;
using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuditCompliance.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuditInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AuditCompliance")
                               ?? configuration.GetConnectionString("Default")
                               ?? "Server=sqlserver;Database=AuditCompliance;User=sa;Password=Your_password123;TrustServerCertificate=True";

        services.AddDbContext<AuditDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IAuditDbContext>(sp => sp.GetRequiredService<AuditDbContext>());
        services.AddScoped<IUnitOfWork, EfUnitOfWork<AuditDbContext>>();

        return services;
    }
}
