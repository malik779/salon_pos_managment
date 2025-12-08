using Microsoft.EntityFrameworkCore;
using Notification.Domain.Messages;

namespace Notification.Application.Abstractions;

public interface INotificationDbContext
{
    DbSet<NotificationMessage> Notifications { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
