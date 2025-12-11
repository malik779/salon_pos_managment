using System.Collections.Generic;
using MediatR;
using NotificationsService.Application.Abstractions;
using NotificationsService.Domain;
using FluentValidation;
using Salon.BuildingBlocks.Abstractions;
using Salon.BuildingBlocks.Audit;

namespace NotificationsService.Application.Notifications;

public sealed record NotificationDto(Guid Id, Guid ClientId, string Channel, string TemplateCode, string Payload, string Status);

public sealed record SendNotificationCommand(Guid ClientId, string Channel, string TemplateCode, Dictionary<string, string> Tokens, string Actor)
    : ICommand<NotificationDto>, IAuditableRequest
{
    public AuditEvent CreateAuditEvent(object response)
    {
        return new AuditEvent(
            Service: "notifications-service",
            Action: "NotificationQueued",
            Actor: Actor,
            OccurredOnUtc: DateTime.UtcNow,
            Metadata: new Dictionary<string, object?>
            {
                ["clientId"] = ClientId,
                ["channel"] = Channel,
                ["template"] = TemplateCode
            });
    }
}

public sealed class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.Channel).NotEmpty();
        RuleFor(x => x.TemplateCode).NotEmpty();
    }
}

public sealed class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, NotificationDto>
{
    private readonly INotificationRepository _repository;

    public SendNotificationCommandHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<NotificationDto> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var payload = System.Text.Json.JsonSerializer.Serialize(request.Tokens);
        var message = new NotificationMessage(Guid.NewGuid(), request.ClientId, request.Channel, request.TemplateCode, payload);
        message.MarkSent();
        await _repository.AddAsync(message, cancellationToken);

        return new NotificationDto(message.Id, message.ClientId, message.Channel, message.TemplateCode, message.Payload, message.Status);
    }
}
