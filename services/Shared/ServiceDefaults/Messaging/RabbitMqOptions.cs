namespace Salon.ServiceDefaults.Messaging;

public sealed class RabbitMqOptions
{
    public string HostName { get; init; } = "rabbitmq";
    public string UserName { get; init; } = "guest";
    public string Password { get; init; } = "guest";
    public string VirtualHost { get; init; } = "/";
    public int Port { get; init; } = 5672;
}
