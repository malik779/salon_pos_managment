using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentsAdapter.Application.Abstractions;
using PaymentsAdapter.Infrastructure.Persistence;

namespace PaymentsAdapter.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PaymentsAdapter")
                               ?? configuration.GetConnectionString("Default")
                               ?? "Server=sqlserver;Database=PaymentsAdapter;User=sa;Password=Your_password123;TrustServerCertificate=True";

        services.AddDbContext<PaymentsDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IPaymentsDbContext>(sp => sp.GetRequiredService<PaymentsDbContext>());
        services.AddScoped<IUnitOfWork, EfUnitOfWork<PaymentsDbContext>>();

        return services;
    }
}
