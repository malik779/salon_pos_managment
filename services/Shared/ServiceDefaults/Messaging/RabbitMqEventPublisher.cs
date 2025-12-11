using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Salon.BuildingBlocks.Messaging;

namespace Salon.ServiceDefaults.Messaging;

public sealed class RabbitMqEventPublisher : IEventPublisher, IDisposable
{
    private readonly ILogger<RabbitMqEventPublisher> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqEventPublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqEventPublisher> logger)
    {
        _logger = logger;
        var configuration = options.Value;
        var factory = new ConnectionFactory
        {
            HostName = configuration.HostName,
            UserName = configuration.UserName,
            Password = configuration.Password,
            VirtualHost = configuration.VirtualHost,
            Port = configuration.Port,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public Task PublishAsync<T>(string exchange, string routingKey, T payload, CancellationToken cancellationToken = default)
    {
        _channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true, autoDelete: false);
        var body = JsonSerializer.SerializeToUtf8Bytes(payload);
        var properties = _channel.CreateBasicProperties();
        properties.ContentType = "application/json";
        properties.DeliveryMode = 2;

        _channel.BasicPublish(exchange, routingKey, properties, body);
        _logger.LogInformation("Published event to {Exchange} with routing key {RoutingKey}", exchange, routingKey);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}
