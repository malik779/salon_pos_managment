namespace BuildingBlocks.Messaging.Options;

public sealed class MessagingOptions
{
    public const string SectionName = "Messaging";
    public string Transport { get; set; } = "RabbitMQ";
    public string HostName { get; set; } = "rabbitmq";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
}
