using MediatR;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("audit-service");

builder.Services.AddSingleton<AuditStore>();
builder.Services.AddMediatR(typeof(Program));

var app = builder.Build();
app.UseServiceDefaults();

app.MapPost("/audit/events", async (CreateAuditEventCommand command, IMediator mediator) => Results.Created("/audit/events", await mediator.Send(command)));
app.MapGet("/audit/events", (AuditStore store) => Results.Ok(store.Events));
app.MapGet("/audit/exports/{id:guid}", (Guid id) => Results.Ok(new { exportId = id, status = "Completed" }));

app.Run();

record AuditEvent(Guid Id, string Actor, string Action, DateTime TimestampUtc, string EntityType, string EntityId);
record CreateAuditEventCommand(string Actor, string Action, string EntityType, string EntityId) : IRequest<AuditEvent>;

class AuditStore
{
    public List<AuditEvent> Events { get; } = new();
}

class CreateAuditEventHandler : IRequestHandler<CreateAuditEventCommand, AuditEvent>
{
    private readonly AuditStore _store;

    public CreateAuditEventHandler(AuditStore store)
    {
        _store = store;
    }

    public Task<AuditEvent> Handle(CreateAuditEventCommand request, CancellationToken cancellationToken)
    {
        var audit = new AuditEvent(Guid.NewGuid(), request.Actor, request.Action, DateTime.UtcNow, request.EntityType, request.EntityId);
        _store.Events.Add(audit);
        return Task.FromResult(audit);
    }
}
