namespace Salon.BuildingBlocks.Cache;

public sealed class NoOpCacheInvalidator : ICacheInvalidator
{
    public Task InvalidateAsync(IReadOnlyCollection<string> keys, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
