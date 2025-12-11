using MediatR;
using Salon.BuildingBlocks.Abstractions;
using Salon.BuildingBlocks.Cache;

namespace Salon.BuildingBlocks.Behaviors;

public sealed class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICacheInvalidator _cacheInvalidator;

    public CacheInvalidationBehavior(ICacheInvalidator cacheInvalidator)
    {
        _cacheInvalidator = cacheInvalidator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        if (request is ICacheInvalidationRequest invalidationRequest && invalidationRequest.CacheKeys.Count > 0)
        {
            await _cacheInvalidator.InvalidateAsync(invalidationRequest.CacheKeys, cancellationToken);
        }

        return response;
    }
}
