using NotificationsService.Domain.Entities;

namespace NotificationsService.Application.Abstractions;

public interface INotificationRepository
{
    Task AddTemplateAsync(NotificationTemplate template, CancellationToken cancellationToken);
    Task<IReadOnlyList<NotificationTemplate>> ListTemplatesAsync(CancellationToken cancellationToken);
}
