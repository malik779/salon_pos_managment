using BuildingBlocks.Application.Contracts;

namespace BuildingBlocks.Infrastructure.Time;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
