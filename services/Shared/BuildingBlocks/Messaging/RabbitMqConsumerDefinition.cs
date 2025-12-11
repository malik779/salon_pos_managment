namespace Salon.BuildingBlocks.Messaging;

public sealed record RabbitMqConsumerDefinition(string Exchange, string Queue, string RoutingKey);
