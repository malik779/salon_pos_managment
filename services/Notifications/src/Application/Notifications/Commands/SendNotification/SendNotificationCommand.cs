using FluentValidation;
using MediatR;
using NotificationsService.Application.Notifications.Models;
using Salon.ServiceDefaults.Messaging;

namespace NotificationsService.Application.Notifications.Commands.SendNotification;

public sealed record SendNotificationCommand(string Channel, string Target, string TemplateName, Dictionary<string, string> Variables) : IRequest<NotificationDispatchDto>;

public sealed class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        RuleFor(x => x.Channel).NotEmpty();
        RuleFor(x => x.Target).NotEmpty();
        RuleFor(x => x.TemplateName).NotEmpty();
    }
}

public sealed class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, NotificationDispatchDto>
{
    private readonly IEventPublisher _eventPublisher;

    public SendNotificationCommandHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task<NotificationDispatchDto> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var dispatch = new NotificationDispatchDto(Guid.NewGuid(), request.Channel, request.Target, "Queued", request.TemplateName);
        await _eventPublisher.PublishAsync("notifications", "notifications.dispatch", dispatch, cancellationToken);
        return dispatch;
    }
}
