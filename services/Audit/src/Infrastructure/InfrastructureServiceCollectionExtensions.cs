using AuditService.Application.Abstractions;
using AuditService.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Salon.BuildingBlocks.Extensions;

namespace AuditService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddAuditInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceDbContext<AuditDbContext>(configuration);
        services.AddScoped<IAuditRepository, EfAuditRepository>();
        return services;
    }
}
