using System.Collections.Generic;

namespace Salon.BuildingBlocks.Audit;

public record AuditEvent(
    string Service,
    string Action,
    string Actor,
    DateTime OccurredOnUtc,
    IReadOnlyDictionary<string, object?> Metadata);
