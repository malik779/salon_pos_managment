namespace BuildingBlocks.Application.Contracts;

public interface IAuditLogger
{
    Task WriteAsync(string action, object payload, CancellationToken cancellationToken = default);
}
