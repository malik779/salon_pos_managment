using BranchConfiguration.Application.Abstractions;
using BranchConfiguration.Infrastructure.Persistence;
using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BranchConfiguration.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBranchConfigurationInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BranchConfiguration")
                               ?? configuration.GetConnectionString("Default")
                               ?? "Server=sqlserver;Database=BranchConfiguration;User=sa;Password=Your_password123;TrustServerCertificate=True";

        services.AddDbContext<BranchConfigurationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IBranchConfigurationDbContext>(sp => sp.GetRequiredService<BranchConfigurationDbContext>());
        services.AddScoped<IUnitOfWork, EfUnitOfWork<BranchConfigurationDbContext>>();

        return services;
    }
}
