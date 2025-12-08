namespace BuildingBlocks.Application.Contracts;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
