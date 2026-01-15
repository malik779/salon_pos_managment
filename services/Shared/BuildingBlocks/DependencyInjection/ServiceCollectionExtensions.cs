using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Salon.BuildingBlocks.Abstractions;
using Salon.BuildingBlocks.Audit;
using Salon.BuildingBlocks.Behaviors;
using Salon.BuildingBlocks.Cache;
using Salon.BuildingBlocks.Messaging;
using Microsoft.Extensions.Options;


namespace Salon.BuildingBlocks.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, Assembly assembly)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RetryPolicyBehavior<,>));

        services.AddSingleton<ICacheInvalidator, NoOpCacheInvalidator>();
        services.AddSingleton<IRetryPolicyProvider, DefaultRetryPolicyProvider>();

        return services;
    }

    public static IServiceCollection AddMessagingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
    .AddOptions<RabbitMqOptions>().Bind(configuration.GetSection("RabbitMQ")).ValidateDataAnnotations().ValidateOnStart();
        services.AddSingleton<RabbitMqPublisher>();
        services.AddSingleton<IIntegrationEventPublisher>(sp => sp.GetRequiredService<RabbitMqPublisher>());
        services.AddSingleton<IAuditEventPublisher>(sp => sp.GetRequiredService<RabbitMqPublisher>());
        
        return services;
    }

    public static IServiceCollection AddRabbitConsumer<TEvent, THandler>(this IServiceCollection services)
        where TEvent : IntegrationEvent
        where THandler : class, IIntegrationEventHandler<TEvent>
    {
        services.AddScoped<THandler>();
        services.AddHostedService<RabbitMqConsumerHostedService<TEvent, THandler>>();
        return services;
    }
}
