using FluentValidation;
using MediatR;
using Salon.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDefaults("sync-service");

builder.Services.AddSingleton<DeviceSyncStore>();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();
app.UseServiceDefaults();

app.MapPost("/sync/register", async (RegisterDeviceCommand command, IMediator mediator) => Results.Ok(await mediator.Send(command)));
app.MapPost("/sync/push", async (PushSyncBatchCommand command, IMediator mediator) => Results.Ok(await mediator.Send(command)));
app.MapGet("/sync/pull", (Guid deviceId, long lastSequence, DeviceSyncStore store) => Results.Ok(store.Pull(deviceId, lastSequence)));

app.Run();

record DeviceRegistration(Guid DeviceId, string SyncToken, DateTime ExpiresAtUtc);
record SyncOperation(long Sequence, string OperationType, string Payload);
record RegisterDeviceCommand(Guid DeviceId, string Platform, string PublicKey) : IRequest<DeviceRegistration>;
record PushSyncBatchCommand(Guid DeviceId, string SyncToken, List<SyncOperation> Operations) : IRequest<long>;

class RegisterDeviceValidator : AbstractValidator<RegisterDeviceCommand>
{
    public RegisterDeviceValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty();
        RuleFor(x => x.PublicKey).NotEmpty();
    }
}

class PushSyncBatchValidator : AbstractValidator<PushSyncBatchCommand>
{
    public PushSyncBatchValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty();
        RuleFor(x => x.SyncToken).NotEmpty();
    }
}

class DeviceSyncStore
{
    private readonly Dictionary<Guid, DeviceRegistration> _registrations = new();
    private readonly Dictionary<Guid, List<SyncOperation>> _operations = new();

    public DeviceRegistration Register(Guid deviceId)
    {
        var registration = new DeviceRegistration(deviceId, Guid.NewGuid().ToString("N"), DateTime.UtcNow.AddHours(8));
        _registrations[deviceId] = registration;
        return registration;
    }

    public long AppendBatch(Guid deviceId, IEnumerable<SyncOperation> operations)
    {
        if (!_operations.TryGetValue(deviceId, out var list))
        {
            list = new List<SyncOperation>();
            _operations[deviceId] = list;
        }

        list.AddRange(operations);
        return list.Count;
    }

    public IEnumerable<SyncOperation> Pull(Guid deviceId, long lastSequence)
    {
        if (!_operations.TryGetValue(deviceId, out var list))
        {
            return Enumerable.Empty<SyncOperation>();
        }

        return list.Where(op => op.Sequence > lastSequence).OrderBy(op => op.Sequence);
    }
}

class RegisterDeviceHandler : IRequestHandler<RegisterDeviceCommand, DeviceRegistration>
{
    private readonly DeviceSyncStore _store;

    public RegisterDeviceHandler(DeviceSyncStore store)
    {
        _store = store;
    }

    public Task<DeviceRegistration> Handle(RegisterDeviceCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_store.Register(request.DeviceId));
    }
}

class PushSyncBatchHandler : IRequestHandler<PushSyncBatchCommand, long>
{
    private readonly DeviceSyncStore _store;

    public PushSyncBatchHandler(DeviceSyncStore store)
    {
        _store = store;
    }

    public Task<long> Handle(PushSyncBatchCommand request, CancellationToken cancellationToken)
    {
        var lastSequence = _store.AppendBatch(request.DeviceId, request.Operations);
        return Task.FromResult(lastSequence);
    }
}
