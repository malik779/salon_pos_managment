namespace NotificationsService.Domain;

public class NotificationMessage
{
    private NotificationMessage()
    {
        Channel = string.Empty;
        TemplateCode = string.Empty;
        Status = string.Empty;
    }

    public NotificationMessage(Guid id, Guid clientId, string channel, string templateCode, string payload)
    {
        Id = id;
        ClientId = clientId;
        Channel = channel;
        TemplateCode = templateCode;
        Payload = payload;
        Status = "Queued";
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; }
    public string Channel { get; private set; }
    public string TemplateCode { get; private set; }
    public string Payload { get; private set; } = string.Empty;
    public string Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public void MarkSent()
    {
        Status = "Sent";
    }
}
