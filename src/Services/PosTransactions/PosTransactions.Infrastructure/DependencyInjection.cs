using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PosTransactions.Application.Abstractions;
using PosTransactions.Infrastructure.Persistence;

namespace PosTransactions.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPosInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PosTransactions")
                               ?? configuration.GetConnectionString("Default")
                               ?? "Server=sqlserver;Database=PosTransactions;User=sa;Password=Your_password123;TrustServerCertificate=True";

        services.AddDbContext<PosDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IPosDbContext>(sp => sp.GetRequiredService<PosDbContext>());
        services.AddScoped<IUnitOfWork, EfUnitOfWork<PosDbContext>>();

        return services;
    }
}
