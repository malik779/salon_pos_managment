using BuildingBlocks.Application.Contracts;
using BuildingBlocks.Domain.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BuildingBlocks.Application.Behaviors;

public sealed class AuditBehavior<TRequest, TResponse>(IAuditLogger auditLogger, ILogger<AuditBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        try
        {
            var payload = new
            {
                Request = request,
                Response = new { response.IsSuccess, response.Error }
            };

            await auditLogger.WriteAsync(typeof(TRequest).Name, payload, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to write audit record for {Request}", typeof(TRequest).Name);
        }

        return response;
    }
}
