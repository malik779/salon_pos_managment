using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Salon.BuildingBlocks.Extensions;

namespace SyncService.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddSyncApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddDefaultPipelines();
        return services;
    }
}
