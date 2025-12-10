namespace IdentityService.Application.Abstractions;

public interface IDeviceRegistry
{
    Task RegisterAsync(DeviceRegistration registration, CancellationToken cancellationToken);
}

public record DeviceRegistration(Guid DeviceId, string DevicePublicKey, string Platform, string SyncToken, DateTime ExpiresAtUtc);
