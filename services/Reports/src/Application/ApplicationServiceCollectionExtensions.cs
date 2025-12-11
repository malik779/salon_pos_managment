using System.Reflection;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Salon.BuildingBlocks.Extensions;

namespace ReportsService.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddReportsApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddDefaultPipelines();
        return services;
    }
}
