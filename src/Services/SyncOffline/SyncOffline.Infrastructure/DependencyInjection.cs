using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SyncOffline.Application.Abstractions;
using SyncOffline.Infrastructure.Persistence;

namespace SyncOffline.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSyncInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SyncOffline")
                               ?? configuration.GetConnectionString("Default")
                               ?? "Server=sqlserver;Database=SyncOffline;User=sa;Password=Your_password123;TrustServerCertificate=True";

        services.AddDbContext<SyncDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<ISyncDbContext>(sp => sp.GetRequiredService<SyncDbContext>());
        services.AddScoped<IUnitOfWork, EfUnitOfWork<SyncDbContext>>();

        return services;
    }
}
