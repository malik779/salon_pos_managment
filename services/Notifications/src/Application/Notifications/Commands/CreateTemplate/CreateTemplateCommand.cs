using FluentValidation;
using MediatR;
using NotificationsService.Application.Abstractions;
using NotificationsService.Application.Notifications.Models;
using NotificationsService.Domain.Entities;

namespace NotificationsService.Application.Notifications.Commands.CreateTemplate;

public sealed record CreateTemplateCommand(string Name, string Channel, string Content) : IRequest<NotificationTemplateDto>;

public sealed class CreateTemplateCommandValidator : AbstractValidator<CreateTemplateCommand>
{
    public CreateTemplateCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Channel).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
    }
}

public sealed class CreateTemplateCommandHandler : IRequestHandler<CreateTemplateCommand, NotificationTemplateDto>
{
    private readonly INotificationRepository _repository;

    public CreateTemplateCommandHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<NotificationTemplateDto> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = new NotificationTemplate(Guid.NewGuid(), request.Name, request.Channel, request.Content);
        await _repository.AddTemplateAsync(template, cancellationToken);
        return new NotificationTemplateDto(template.Id, template.Name, template.Channel, template.Content);
    }
}
