using FluentValidation;
using MediatR;
using SyncService.Application.Abstractions;
using SyncService.Domain.Entities;

namespace SyncService.Application.Sync.Commands.PushSyncBatch;

public sealed record PushSyncBatchCommand(Guid DeviceId, string SyncToken, List<SyncOperation> Operations) : IRequest<long>;

public sealed class PushSyncBatchCommandValidator : AbstractValidator<PushSyncBatchCommand>
{
    public PushSyncBatchCommandValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty();
        RuleFor(x => x.SyncToken).NotEmpty();
    }
}

public sealed class PushSyncBatchCommandHandler : IRequestHandler<PushSyncBatchCommand, long>
{
    private readonly IDeviceSyncStore _store;

    public PushSyncBatchCommandHandler(IDeviceSyncStore store)
    {
        _store = store;
    }

    public Task<long> Handle(PushSyncBatchCommand request, CancellationToken cancellationToken)
    {
        return _store.AppendOperationsAsync(request.DeviceId, request.Operations, cancellationToken);
    }
}
