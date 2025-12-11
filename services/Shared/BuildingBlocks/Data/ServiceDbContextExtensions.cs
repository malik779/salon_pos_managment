using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Salon.BuildingBlocks.Data;

public static class ServiceDbContextExtensions
{
    public static IServiceCollection AddServiceDbContext<TContext>(this IServiceCollection services, IConfiguration configuration, string connectionStringName)
        where TContext : ServiceDbContextBase
    {
        var connectionString = configuration.GetConnectionString(connectionStringName);

        services.AddDbContext<TContext>((sp, options) =>
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                options.UseInMemoryDatabase(connectionStringName + "-inmemory");
            }
        });

        services.AddScoped<Abstractions.IUnitOfWork>(sp => sp.GetRequiredService<TContext>());

        return services;
    }
}
