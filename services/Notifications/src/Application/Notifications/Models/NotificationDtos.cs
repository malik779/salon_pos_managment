namespace NotificationsService.Application.Notifications.Models;

public sealed record NotificationTemplateDto(Guid Id, string Name, string Channel, string Content);
public sealed record NotificationDispatchDto(Guid Id, string Channel, string Target, string Status, string TemplateName);
