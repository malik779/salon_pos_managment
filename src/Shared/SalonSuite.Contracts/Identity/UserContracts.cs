namespace SalonSuite.Contracts.Identity;

public sealed record UserSummary(Guid Id, string Email, string Name, Guid DefaultBranchId, bool IsActive);

public sealed record RoleSummary(Guid Id, string Name);
