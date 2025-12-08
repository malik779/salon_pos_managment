using BuildingBlocks.Domain.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {Request}...", typeof(TRequest).Name);
        var response = await next();
        if (response.IsSuccess)
        {
            logger.LogInformation("Handled {Request} successfully", typeof(TRequest).Name);
        }
        else
        {
            logger.LogWarning("Request {Request} failed: {Error}", typeof(TRequest).Name, response.Error.Message);
        }

        return response;
    }
}
