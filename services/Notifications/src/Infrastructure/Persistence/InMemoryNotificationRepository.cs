using NotificationsService.Application.Abstractions;
using NotificationsService.Domain.Entities;

namespace NotificationsService.Infrastructure.Persistence;

public sealed class InMemoryNotificationRepository : INotificationRepository
{
    private readonly List<NotificationTemplate> _templates = new();

    public Task AddTemplateAsync(NotificationTemplate template, CancellationToken cancellationToken)
    {
        _templates.Add(template);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<NotificationTemplate>> ListTemplatesAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<NotificationTemplate> snapshot = _templates.ToList();
        return Task.FromResult(snapshot);
    }
}
