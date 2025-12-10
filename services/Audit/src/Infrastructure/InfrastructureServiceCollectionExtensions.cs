using AuditService.Application.Abstractions;
using AuditService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace AuditService.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddAuditInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IAuditRepository, InMemoryAuditRepository>();
        return services;
    }
}
