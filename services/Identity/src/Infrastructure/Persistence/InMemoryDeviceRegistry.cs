using IdentityService.Application.Abstractions;

namespace IdentityService.Infrastructure.Persistence;

public sealed class InMemoryDeviceRegistry : IDeviceRegistry
{
    private readonly List<DeviceRegistration> _registrations = new();

    public Task RegisterAsync(DeviceRegistration registration, CancellationToken cancellationToken)
    {
        _registrations.RemoveAll(x => x.DeviceId == registration.DeviceId);
        _registrations.Add(registration);
        return Task.CompletedTask;
    }
}
