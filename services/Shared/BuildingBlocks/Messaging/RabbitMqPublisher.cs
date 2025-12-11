using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Salon.BuildingBlocks.Audit;

namespace Salon.BuildingBlocks.Messaging;

public sealed class RabbitMqPublisher : IIntegrationEventPublisher, IAuditEventPublisher, IDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly Lazy<IConnection> _connection;

    public RabbitMqPublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;
        _connection = new Lazy<IConnection>(CreateConnection);
    }

    public Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent
    {
        PublishInternal(integrationEvent, typeof(TEvent).Name);
        return Task.CompletedTask;
    }

    public Task PublishAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default)
    {
        PublishInternal(auditEvent, "audit.events");
        return Task.CompletedTask;
    }

    private void PublishInternal<T>(T payloadModel, string routingKey)
    {
        var payload = JsonSerializer.SerializeToUtf8Bytes(payloadModel, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        try
        {
            using var channel = _connection.Value.CreateModel();
            channel.ExchangeDeclare(exchange: _options.Exchange, type: ExchangeType.Topic, durable: true);
            var props = channel.CreateBasicProperties();
            props.DeliveryMode = 2;
            channel.BasicPublish(exchange: _options.Exchange, routingKey: routingKey, basicProperties: props, body: payload);
            _logger.LogInformation("Published payload {Type} with routing key {RoutingKey}", typeof(T).Name, routingKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed publishing payload {Type}", typeof(T).Name);
            throw;
        }
    }

    private IConnection CreateConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            DispatchConsumersAsync = true
        };

        return factory.CreateConnection();
    }

    public void Dispose()
    {
        if (_connection.IsValueCreated)
        {
            _connection.Value.Dispose();
        }
    }
}
