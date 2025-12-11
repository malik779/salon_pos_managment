using MediatR;
using Salon.BuildingBlocks.Abstractions;
using SyncService.Application.Abstractions;

namespace SyncService.Application.Devices;

public sealed record GetDeviceSyncStateQuery(Guid DeviceId) : IQuery<DeviceSyncDto?>;

public sealed class GetDeviceSyncStateQueryHandler : IRequestHandler<GetDeviceSyncStateQuery, DeviceSyncDto?>
{
    private readonly IDeviceSyncRepository _repository;

    public GetDeviceSyncStateQueryHandler(IDeviceSyncRepository repository)
    {
        _repository = repository;
    }

    public async Task<DeviceSyncDto?> Handle(GetDeviceSyncStateQuery request, CancellationToken cancellationToken)
    {
        var state = await _repository.GetAsync(request.DeviceId, cancellationToken);
        return state is null
            ? null
            : new DeviceSyncDto(state.DeviceId, state.Platform, state.LastSyncedAtUtc, state.Sequence);
    }
}
