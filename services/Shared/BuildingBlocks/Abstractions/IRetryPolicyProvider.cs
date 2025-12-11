namespace Salon.BuildingBlocks.Abstractions;

public interface IRetryPolicyProvider
{
    Task<TResponse> ExecuteAsync<TResponse>(Func<Task<TResponse>> action, int retryCount, CancellationToken cancellationToken);
}
