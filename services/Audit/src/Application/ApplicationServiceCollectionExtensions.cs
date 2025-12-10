using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AuditService.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddAuditApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        return services;
    }
}
