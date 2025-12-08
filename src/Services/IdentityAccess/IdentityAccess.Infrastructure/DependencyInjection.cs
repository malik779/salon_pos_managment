using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Infrastructure.Persistence;
using IdentityAccess.Application.Abstractions;
using IdentityAccess.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityAccess.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityAccessInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentityAccess")
                               ?? configuration.GetConnectionString("Default")
                               ?? "Server=sqlserver;Database=IdentityAccess;User=sa;Password=Your_password123;TrustServerCertificate=True";

        services.AddDbContext<IdentityAccessDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IIdentityAccessDbContext>(sp => sp.GetRequiredService<IdentityAccessDbContext>());
        services.AddScoped<IUnitOfWork, EfUnitOfWork<IdentityAccessDbContext>>();

        return services;
    }
}
