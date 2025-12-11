using NotificationsService.Domain;

namespace NotificationsService.Application.Abstractions;

public interface INotificationRepository
{
    Task<NotificationMessage?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(NotificationMessage message, CancellationToken cancellationToken);
}
