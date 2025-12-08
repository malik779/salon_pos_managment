using System.Reflection;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Contracts;
using BuildingBlocks.Application.Persistence;
using BuildingBlocks.Infrastructure.Auditing;
using BuildingBlocks.Infrastructure.Caching;
using BuildingBlocks.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureBuildingBlocks(
        this IServiceCollection services,
        IConfiguration configuration,
        Assembly[] assemblies)
    {
        var redisConnection = configuration.GetConnectionString("Redis") ?? "redis:6379";

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
        });

        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IAuditLogger, SerilogAuditLogger>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        services.AddApplicationBuildingBlocks(assemblies);
        return services;
    }
}
