namespace BuildingBlocks.Application.Contracts;

public interface ICurrentUserContext
{
    Guid UserId { get; }
    string? Email { get; }
    string? DisplayName { get; }
    IReadOnlyCollection<string> Roles { get; }
    string? BranchId { get; }
}
