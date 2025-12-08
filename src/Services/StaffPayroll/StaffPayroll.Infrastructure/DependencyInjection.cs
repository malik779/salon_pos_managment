using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StaffPayroll.Application.Abstractions;
using StaffPayroll.Infrastructure.Persistence;

namespace StaffPayroll.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddStaffPayrollInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("StaffPayroll")
                               ?? configuration.GetConnectionString("Default")
                               ?? "Server=sqlserver;Database=StaffPayroll;User=sa;Password=Your_password123;TrustServerCertificate=True";

        services.AddDbContext<StaffDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IStaffDbContext>(sp => sp.GetRequiredService<StaffDbContext>());
        services.AddScoped<IUnitOfWork, EfUnitOfWork<StaffDbContext>>();

        return services;
    }
}
