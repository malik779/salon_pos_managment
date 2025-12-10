namespace SyncService.Domain.Entities;

public sealed record DeviceRegistration(Guid DeviceId, string SyncToken, DateTime ExpiresAtUtc);
public sealed record SyncOperation(long Sequence, string OperationType, string Payload);
