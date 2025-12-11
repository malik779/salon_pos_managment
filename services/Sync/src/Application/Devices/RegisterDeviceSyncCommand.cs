using System.Collections.Generic;
using FluentValidation;
using MediatR;
using Salon.BuildingBlocks.Abstractions;
using Salon.BuildingBlocks.Audit;
using SyncService.Application.Abstractions;
using SyncService.Domain;

namespace SyncService.Application.Devices;

public sealed record DeviceSyncDto(Guid DeviceId, string Platform, DateTime LastSyncedAtUtc, int Sequence);

public sealed record RegisterDeviceSyncCommand(Guid DeviceId, string Platform, string Actor)
    : ICommand<DeviceSyncDto>, IAuditableRequest
{
    public AuditEvent CreateAuditEvent(object response)
    {
        return new AuditEvent(
            Service: "sync-service",
            Action: "DeviceRegistered",
            Actor: Actor,
            OccurredOnUtc: DateTime.UtcNow,
            Metadata: new Dictionary<string, object?>
            {
                ["deviceId"] = DeviceId,
                ["platform"] = Platform
            });
    }
}

public sealed class RegisterDeviceSyncCommandValidator : AbstractValidator<RegisterDeviceSyncCommand>
{
    public RegisterDeviceSyncCommandValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty();
        RuleFor(x => x.Platform).NotEmpty();
    }
}

public sealed class RegisterDeviceSyncCommandHandler : IRequestHandler<RegisterDeviceSyncCommand, DeviceSyncDto>
{
    private readonly IDeviceSyncRepository _repository;

    public RegisterDeviceSyncCommandHandler(IDeviceSyncRepository repository)
    {
        _repository = repository;
    }

    public async Task<DeviceSyncDto> Handle(RegisterDeviceSyncCommand request, CancellationToken cancellationToken)
    {
        var state = await _repository.GetAsync(request.DeviceId, cancellationToken) ?? new DeviceSyncState(request.DeviceId, request.Platform);
        state.MarkSynced();
        await _repository.UpsertAsync(state, cancellationToken);
        return new DeviceSyncDto(state.DeviceId, state.Platform, state.LastSyncedAtUtc, state.Sequence);
    }
}
