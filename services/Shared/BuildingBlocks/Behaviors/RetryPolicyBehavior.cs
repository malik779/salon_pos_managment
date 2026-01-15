using MediatR;
using Salon.BuildingBlocks.Abstractions;

namespace Salon.BuildingBlocks.Behaviors;

public sealed class RetryPolicyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IRetryPolicyProvider _retryPolicyProvider;

    public RetryPolicyBehavior(IRetryPolicyProvider retryPolicyProvider)
    {
        _retryPolicyProvider = retryPolicyProvider;
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not IRetryableRequest retryable || retryable.RetryCount <= 1)
        {
            return next();
        }

        // Wrap the RequestHandlerDelegate<TResponse> in a lambda to match the expected Func<Task<TResponse>> signature
        Func<Task<TResponse>> action = () => next();

        return _retryPolicyProvider.ExecuteAsync(action, retryable.RetryCount, cancellationToken);
    }
}
