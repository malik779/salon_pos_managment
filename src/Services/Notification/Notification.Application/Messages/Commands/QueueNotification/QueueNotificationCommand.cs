using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Notification.Application.Abstractions;
using Notification.Domain.Messages;

namespace Notification.Application.Messages.Commands.QueueNotification;

public sealed record QueueNotificationCommand(Guid ClientId, string Channel, string TemplateCode) : ICommand<Guid>;

public sealed class QueueNotificationCommandValidator : AbstractValidator<QueueNotificationCommand>
{
    public QueueNotificationCommandValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.Channel).NotEmpty().MaximumLength(20);
        RuleFor(x => x.TemplateCode).NotEmpty().MaximumLength(50);
    }
}

public sealed class QueueNotificationCommandHandler(INotificationDbContext context)
    : ICommandHandler<QueueNotificationCommand, Guid>
{
    public async Task<Result<Guid>> Handle(QueueNotificationCommand request, CancellationToken cancellationToken)
    {
        var message = NotificationMessage.Queue(request.ClientId, request.Channel, request.TemplateCode);
        context.Notifications.Add(message);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(message.Id);
    }
}
