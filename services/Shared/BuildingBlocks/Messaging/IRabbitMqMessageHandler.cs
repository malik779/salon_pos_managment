namespace Salon.BuildingBlocks.Messaging;

public interface IRabbitMqMessageHandler<TMessage>
{
    Task HandleAsync(TMessage message, CancellationToken cancellationToken);
}
