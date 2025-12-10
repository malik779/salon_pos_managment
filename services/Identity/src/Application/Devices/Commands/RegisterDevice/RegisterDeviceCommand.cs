using FluentValidation;
using IdentityService.Application.Abstractions;
using MediatR;

namespace IdentityService.Application.Devices.Commands.RegisterDevice;

public sealed record RegisterDeviceCommand(Guid DeviceId, string DevicePublicKey, string Platform) : IRequest<DeviceRegistrationResponse>;

public sealed record DeviceRegistrationResponse(Guid DeviceId, string SyncToken, int ExpiresInSeconds);

public sealed class RegisterDeviceCommandValidator : AbstractValidator<RegisterDeviceCommand>
{
    public RegisterDeviceCommandValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty();
        RuleFor(x => x.DevicePublicKey).NotEmpty();
        RuleFor(x => x.Platform).NotEmpty();
    }
}

public sealed class RegisterDeviceCommandHandler : IRequestHandler<RegisterDeviceCommand, DeviceRegistrationResponse>
{
    private readonly IDeviceRegistry _registry;

    public RegisterDeviceCommandHandler(IDeviceRegistry registry)
    {
        _registry = registry;
    }

    public async Task<DeviceRegistrationResponse> Handle(RegisterDeviceCommand request, CancellationToken cancellationToken)
    {
        var syncToken = Guid.NewGuid().ToString("N");
        var registration = new DeviceRegistration(
            request.DeviceId,
            request.DevicePublicKey,
            request.Platform,
            syncToken,
            DateTime.UtcNow.AddHours(8));

        await _registry.RegisterAsync(registration, cancellationToken);

        return new DeviceRegistrationResponse(request.DeviceId, syncToken, 8 * 3600);
    }
}
