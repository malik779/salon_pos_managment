namespace NotificationsService.Domain.Entities;

public sealed class NotificationTemplate
{
    public NotificationTemplate(Guid id, string name, string channel, string content)
    {
        Id = id;
        Name = name;
        Channel = channel;
        Content = content;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Channel { get; }
    public string Content { get; }
}
