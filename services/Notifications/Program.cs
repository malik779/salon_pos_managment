using FluentValidation;
using MediatR;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("notifications-service");

builder.Services.AddSingleton<NotificationStore>();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();
app.UseServiceDefaults();

app.MapPost("/notifications/send", async (SendNotificationCommand command, IMediator mediator) => Results.Ok(await mediator.Send(command)));
app.MapPost("/templates", async (CreateTemplateCommand command, IMediator mediator) => Results.Created("/templates", await mediator.Send(command)));
app.MapGet("/templates", (NotificationStore store) => Results.Ok(store.Templates));

app.Run();

record NotificationTemplate(Guid Id, string Name, string Channel, string Content);
record NotificationDispatch(Guid Id, string Channel, string Target, string Status, string TemplateName);

record CreateTemplateCommand(string Name, string Channel, string Content) : IRequest<NotificationTemplate>;
record SendNotificationCommand(string Channel, string Target, string TemplateName, Dictionary<string, string> Variables) : IRequest<NotificationDispatch>;

class CreateTemplateValidator : AbstractValidator<CreateTemplateCommand>
{
    public CreateTemplateValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Channel).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
    }
}

class SendNotificationValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationValidator()
    {
        RuleFor(x => x.Channel).NotEmpty();
        RuleFor(x => x.Target).NotEmpty();
        RuleFor(x => x.TemplateName).NotEmpty();
    }
}

class NotificationStore
{
    public List<NotificationTemplate> Templates { get; } = new();

    public NotificationTemplate? FindTemplate(string name) => Templates.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
}

class CreateTemplateHandler : IRequestHandler<CreateTemplateCommand, NotificationTemplate>
{
    private readonly NotificationStore _store;

    public CreateTemplateHandler(NotificationStore store)
    {
        _store = store;
    }

    public Task<NotificationTemplate> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = new NotificationTemplate(Guid.NewGuid(), request.Name, request.Channel, request.Content);
        _store.Templates.Add(template);
        return Task.FromResult(template);
    }
}

class SendNotificationHandler : IRequestHandler<SendNotificationCommand, NotificationDispatch>
{
    private readonly NotificationStore _store;

    public SendNotificationHandler(NotificationStore store)
    {
        _store = store;
    }

    public Task<NotificationDispatch> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var template = _store.FindTemplate(request.TemplateName) ?? throw new KeyNotFoundException("template_not_found");
        var dispatch = new NotificationDispatch(Guid.NewGuid(), request.Channel, request.Target, "Queued", template.Name);
        return Task.FromResult(dispatch);
    }
}
