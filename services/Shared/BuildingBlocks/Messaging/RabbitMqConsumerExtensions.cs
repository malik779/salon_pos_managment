using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Salon.BuildingBlocks.Messaging;

public static class RabbitMqConsumerExtensions
{
    public static IServiceCollection AddRabbitConsumer<THandler, TMessage>(this IServiceCollection services, RabbitMqConsumerDefinition definition)
        where THandler : class, IRabbitMqMessageHandler<TMessage>
    {
        services.AddScoped<IRabbitMqMessageHandler<TMessage>, THandler>();
        services.AddHostedService(sp => new RabbitMqConsumerHostedService<TMessage>(
            sp,
            definition,
            sp.GetRequiredService<IOptions<RabbitMqOptions>>(),
            sp.GetRequiredService<ILogger<RabbitMqConsumerHostedService<TMessage>>>()));
        return services;
    }
}
