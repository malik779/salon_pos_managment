using FluentValidation;
using MediatR;
using SyncService.Application.Abstractions;
using SyncService.Domain.Entities;

namespace SyncService.Application.Sync.Commands.RegisterDevice;

public sealed record RegisterDeviceCommand(Guid DeviceId, string Platform, string PublicKey) : IRequest<DeviceRegistration>;

public sealed class RegisterDeviceCommandValidator : AbstractValidator<RegisterDeviceCommand>
{
    public RegisterDeviceCommandValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty();
        RuleFor(x => x.Platform).NotEmpty();
        RuleFor(x => x.PublicKey).NotEmpty();
    }
}

public sealed class RegisterDeviceCommandHandler : IRequestHandler<RegisterDeviceCommand, DeviceRegistration>
{
    private readonly IDeviceSyncStore _store;

    public RegisterDeviceCommandHandler(IDeviceSyncStore store)
    {
        _store = store;
    }

    public Task<DeviceRegistration> Handle(RegisterDeviceCommand request, CancellationToken cancellationToken)
    {
        return _store.RegisterAsync(request.DeviceId, request.Platform, request.PublicKey, cancellationToken);
    }
}
