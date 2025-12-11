using System;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using Salon.BuildingBlocks.Abstractions;

namespace Salon.BuildingBlocks.DependencyInjection;

internal sealed class DefaultRetryPolicyProvider : IRetryPolicyProvider
{
    public async Task<TResponse> ExecuteAsync<TResponse>(Func<Task<TResponse>> action, int retryCount, CancellationToken cancellationToken)
    {
        var policy = CreatePolicy<TResponse>(retryCount);
        return await policy.ExecuteAsync(ct => action(), cancellationToken);
    }

    private static AsyncRetryPolicy<TResponse> CreatePolicy<TResponse>(int retryCount)
    {
        return Policy<TResponse>
            .Handle<Exception>()
            .WaitAndRetryAsync(Math.Max(1, retryCount), attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt - 1)));
    }
}
