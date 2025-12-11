namespace NotificationsService.Domain.Entities;

public sealed class NotificationTemplate
{
    private NotificationTemplate()
    {
        Name = string.Empty;
        Channel = string.Empty;
        Content = string.Empty;
    }

    public NotificationTemplate(Guid id, string name, string channel, string content)
    {
        Id = id;
        Name = name;
        Channel = channel;
        Content = content;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Channel { get; private set; }
    public string Content { get; private set; }
}
