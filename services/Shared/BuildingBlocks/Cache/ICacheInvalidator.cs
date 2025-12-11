namespace Salon.BuildingBlocks.Cache;

public interface ICacheInvalidator
{
    Task InvalidateAsync(IReadOnlyCollection<string> keys, CancellationToken cancellationToken = default);
}
