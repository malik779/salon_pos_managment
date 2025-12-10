using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ReportsService.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddReportsApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        return services;
    }
}
