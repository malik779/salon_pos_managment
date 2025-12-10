using MediatR;
using NotificationsService.Application.Abstractions;
using NotificationsService.Application.Notifications.Models;

namespace NotificationsService.Application.Notifications.Queries.ListTemplates;

public sealed record ListTemplatesQuery() : IRequest<IReadOnlyList<NotificationTemplateDto>>;

public sealed class ListTemplatesQueryHandler : IRequestHandler<ListTemplatesQuery, IReadOnlyList<NotificationTemplateDto>>
{
    private readonly INotificationRepository _repository;

    public ListTemplatesQueryHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<NotificationTemplateDto>> Handle(ListTemplatesQuery request, CancellationToken cancellationToken)
    {
        var templates = await _repository.ListTemplatesAsync(cancellationToken);
        return templates.Select(t => new NotificationTemplateDto(t.Id, t.Name, t.Channel, t.Content)).ToList();
    }
}
