using MediatR;
using SyncService.Application.Abstractions;
using SyncService.Domain.Entities;

namespace SyncService.Application.Sync.Queries.PullChanges;

public sealed record PullChangesQuery(Guid DeviceId, long LastSequence) : IRequest<IEnumerable<SyncOperation>>;

public sealed class PullChangesQueryHandler : IRequestHandler<PullChangesQuery, IEnumerable<SyncOperation>>
{
    private readonly IDeviceSyncStore _store;

    public PullChangesQueryHandler(IDeviceSyncStore store)
    {
        _store = store;
    }

    public Task<IEnumerable<SyncOperation>> Handle(PullChangesQuery request, CancellationToken cancellationToken)
    {
        return _store.PullAsync(request.DeviceId, request.LastSequence, cancellationToken);
    }
}
