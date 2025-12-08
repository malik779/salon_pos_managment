using BuildingBlocks.Domain.Abstractions;

namespace Notification.Domain.Messages;

public sealed class NotificationMessage : AuditableEntity, IAggregateRoot
{
    private NotificationMessage()
    {
    }

    private NotificationMessage(Guid id, Guid clientId, string channel, string templateCode)
    {
        Id = id;
        ClientId = clientId;
        Channel = channel;
        TemplateCode = templateCode;
        Status = "Queued";
        QueuedAtUtc = DateTime.UtcNow;
    }

    public Guid ClientId { get; private set; }
        = Guid.Empty;
    public string Channel { get; private set; } = default!;
    public string TemplateCode { get; private set; } = default!;
    public string Status { get; private set; } = "Queued";
    public DateTime QueuedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? SentAtUtc { get; private set; }
        = null;

    public static NotificationMessage Queue(Guid clientId, string channel, string templateCode)
        => new(Guid.NewGuid(), clientId, channel, templateCode);
}
