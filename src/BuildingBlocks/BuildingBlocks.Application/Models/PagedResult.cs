namespace BuildingBlocks.Application.Models;

public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int PageNumber, int PageSize, long TotalCount)
{
    public static PagedResult<T> Empty(int pageNumber = 1, int pageSize = 10)
        => new(Array.Empty<T>(), pageNumber, pageSize, 0);
}
