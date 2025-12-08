using BuildingBlocks.Domain.Abstractions;

namespace SyncOffline.Domain.Devices;

public sealed class DeviceRegistration : AuditableEntity, IAggregateRoot
{
    private DeviceRegistration()
    {
    }

    private DeviceRegistration(Guid id, string deviceId, Guid branchId, string platform)
    {
        Id = id;
        DeviceId = deviceId;
        BranchId = branchId;
        Platform = platform;
        SyncToken = Guid.NewGuid().ToString("N");
    }

    public string DeviceId { get; private set; } = default!;
    public Guid BranchId { get; private set; } = Guid.Empty;
    public string Platform { get; private set; } = default!;
    public DateTime? LastSyncUtc { get; private set; }
        = null;
    public string SyncToken { get; private set; } = default!;

    public static DeviceRegistration Register(string deviceId, Guid branchId, string platform)
        => new(Guid.NewGuid(), deviceId, branchId, platform);
}
