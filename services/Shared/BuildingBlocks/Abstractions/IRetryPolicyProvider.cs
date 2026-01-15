using MediatR;

namespace Salon.BuildingBlocks.Abstractions;

public interface IRetryPolicyProvider
{
    Task<TResponse> ExecuteAsync<TResponse>(Func<Task<TResponse>> action, int retryCount, CancellationToken cancellationToken);
    Task<object> ExecuteAsync<T>(RequestHandlerDelegate<object> next, int retryCount, CancellationToken cancellationToken);
}
