using BuildingBlocks.Application.Contracts;
using BuildingBlocks.Application.Models;
using BuildingBlocks.Domain.Primitives;
using MediatR;

namespace BuildingBlocks.Application.Behaviors;

public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan? SlidingExpiration { get; }
}

public sealed class CachingBehavior<TRequest, TResponse>(ICacheService cache)
    : IPipelineBehavior<TRequest, Result<TResponse>>
    where TRequest : ICacheableQuery, IRequest<Result<TResponse>>
{
    public async Task<Result<TResponse>> Handle(
        TRequest request,
        RequestHandlerDelegate<Result<TResponse>> next,
        CancellationToken cancellationToken)
    {
        if (request.SlidingExpiration is null)
        {
            return await next();
        }

        var cached = await cache.GetAsync<TResponse>(request.CacheKey, cancellationToken);
        if (cached is not null)
        {
            return Result.Success(cached);
        }

        var response = await next();
        if (response.IsSuccess)
        {
            await cache.SetAsync(request.CacheKey, response.Value!, request.SlidingExpiration.Value, cancellationToken);
        }

        return response;
    }
}
