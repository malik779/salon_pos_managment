using System;
using System.Collections.Generic;
using MediatR;
using Salon.BuildingBlocks.Audit;

namespace Salon.BuildingBlocks.Abstractions;

public interface ICommand<out TResponse> : IRequest<TResponse>, ITransactionalRequest { }

public interface IQuery<out TResponse> : IRequest<TResponse> { }

public interface ITransactionalRequest { }

public interface IAuditableRequest
{
    AuditEvent CreateAuditEvent(object response);
}

public interface ICacheInvalidationRequest
{
    IReadOnlyCollection<string> CacheKeys => Array.Empty<string>();
}

public interface IRetryableRequest
{
    int RetryCount => 1;
}
