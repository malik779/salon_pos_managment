namespace SyncService.Domain;

public class DeviceSyncState
{
    private DeviceSyncState()
    {
        Platform = string.Empty;
    }

    public DeviceSyncState(Guid deviceId, string platform)
    {
        DeviceId = deviceId;
        Platform = platform;
        LastSyncedAtUtc = DateTime.UtcNow;
        Sequence = 0;
    }

    public Guid DeviceId { get; private set; }
    public string Platform { get; private set; }
    public DateTime LastSyncedAtUtc { get; private set; }
    public int Sequence { get; private set; }

    public void MarkSynced()
    {
        Sequence++;
        LastSyncedAtUtc = DateTime.UtcNow;
    }
}
