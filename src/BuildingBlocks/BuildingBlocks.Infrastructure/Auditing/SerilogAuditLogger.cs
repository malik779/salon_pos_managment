using System.Text.Json;
using BuildingBlocks.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Auditing;

public sealed class SerilogAuditLogger(ILogger<SerilogAuditLogger> logger) : IAuditLogger
{
    public Task WriteAsync(string action, object payload, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(payload);
        logger.LogInformation("AUDIT {Action} {Payload}", action, json);
        return Task.CompletedTask;
    }
}
