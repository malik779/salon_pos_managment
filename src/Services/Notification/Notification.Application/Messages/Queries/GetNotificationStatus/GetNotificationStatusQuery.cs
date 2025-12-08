using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using Notification.Application.Abstractions;

namespace Notification.Application.Messages.Queries.GetNotificationStatus;

public sealed record NotificationStatusDto(Guid Id, string Status, DateTime QueuedAtUtc, DateTime? SentAtUtc);

public sealed record GetNotificationStatusQuery(Guid NotificationId) : IQuery<NotificationStatusDto>;

public sealed class GetNotificationStatusQueryHandler(INotificationDbContext context)
    : IQueryHandler<GetNotificationStatusQuery, NotificationStatusDto>
{
    public async Task<Result<NotificationStatusDto>> Handle(GetNotificationStatusQuery request, CancellationToken cancellationToken)
    {
        var message = await context.Notifications
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == request.NotificationId, cancellationToken);

        if (message is null)
        {
            return Result.Failure<NotificationStatusDto>(new Error("Notification.NotificationNotFound", "Notification not found"));
        }

        var dto = new NotificationStatusDto(message.Id, message.Status, message.QueuedAtUtc, message.SentAtUtc);
        return Result.Success(dto);
    }
}
