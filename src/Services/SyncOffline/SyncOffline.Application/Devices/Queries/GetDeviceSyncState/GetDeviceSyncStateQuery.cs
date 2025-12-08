using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using SyncOffline.Application.Abstractions;

namespace SyncOffline.Application.Devices.Queries.GetDeviceSyncState;

public sealed record DeviceSyncStateDto(Guid Id, string DeviceId, Guid BranchId, string Platform, DateTime? LastSyncUtc, string SyncToken);

public sealed record GetDeviceSyncStateQuery(string DeviceId) : IQuery<DeviceSyncStateDto>;

public sealed class GetDeviceSyncStateQueryHandler(ISyncDbContext context)
    : IQueryHandler<GetDeviceSyncStateQuery, DeviceSyncStateDto>
{
    public async Task<Result<DeviceSyncStateDto>> Handle(GetDeviceSyncStateQuery request, CancellationToken cancellationToken)
    {
        var device = await context.Devices.AsNoTracking().FirstOrDefaultAsync(d => d.DeviceId == request.DeviceId, cancellationToken);
        if (device is null)
        {
            return Result.Failure<DeviceSyncStateDto>(new Error("SyncOffline.DeviceNotFound", "Device not registered"));
        }

        var dto = new DeviceSyncStateDto(device.Id, device.DeviceId, device.BranchId, device.Platform, device.LastSyncUtc, device.SyncToken);
        return Result.Success(dto);
    }
}
