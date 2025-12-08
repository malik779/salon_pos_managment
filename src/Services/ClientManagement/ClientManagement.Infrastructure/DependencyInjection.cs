using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Infrastructure.Persistence;
using ClientManagement.Application.Abstractions;
using ClientManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClientManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddClientManagementInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ClientManagement")
                               ?? configuration.GetConnectionString("Default")
                               ?? "Server=sqlserver;Database=ClientManagement;User=sa;Password=Your_password123;TrustServerCertificate=True";

        services.AddDbContext<ClientDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IClientDbContext>(sp => sp.GetRequiredService<ClientDbContext>());
        services.AddScoped<IUnitOfWork, EfUnitOfWork<ClientDbContext>>();

        return services;
    }
}
