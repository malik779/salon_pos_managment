using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Salon.BuildingBlocks.Messaging;

public sealed class RabbitMqConsumerHostedService<TMessage> : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMqConsumerDefinition _definition;
    private readonly IOptions<RabbitMqOptions> _options;
    private readonly ILogger<RabbitMqConsumerHostedService<TMessage>> _logger;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqConsumerHostedService(
        IServiceProvider serviceProvider,
        RabbitMqConsumerDefinition definition,
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqConsumerHostedService<TMessage>> logger)
    {
        _serviceProvider = serviceProvider;
        _definition = definition;
        _options = options;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.Value.HostName,
            UserName = _options.Value.UserName,
            Password = _options.Value.Password,
            VirtualHost = _options.Value.VirtualHost,
            Port = _options.Value.Port,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_definition.Exchange, ExchangeType.Topic, durable: true);
        _channel.QueueDeclare(_definition.Queue, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(_definition.Queue, _definition.Exchange, _definition.RoutingKey);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, args) =>
        {
            try
            {
                var payload = JsonSerializer.Deserialize<TMessage>(Encoding.UTF8.GetString(args.Body.Span));
                if (payload is null)
                {
                    _channel!.BasicAck(args.DeliveryTag, multiple: false);
                    return;
                }

                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IRabbitMqMessageHandler<TMessage>>();
                await handler.HandleAsync(payload, stoppingToken);
                _channel!.BasicAck(args.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle message {MessageId}", args.BasicProperties.MessageId);
                _channel!.BasicNack(args.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(_definition.Queue, autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
