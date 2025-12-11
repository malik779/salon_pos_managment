using MediatR;
using Microsoft.Extensions.Configuration;
using Salon.BuildingBlocks.Abstractions;
using Salon.BuildingBlocks.Audit;

namespace Salon.BuildingBlocks.Behaviors;

public sealed class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IAuditEventPublisher _auditPublisher;
    private readonly string _serviceName;

    public AuditBehavior(IAuditEventPublisher auditPublisher, IConfiguration configuration)
    {
        _auditPublisher = auditPublisher;
        _serviceName = configuration["Service:Name"] ?? "unknown-service";
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        if (request is IAuditableRequest auditable)
        {
            var auditEvent = auditable.CreateAuditEvent(response);
            auditEvent = auditEvent with { Service = _serviceName };
            await _auditPublisher.PublishAsync(auditEvent, cancellationToken);
        }

        return response;
    }
}
