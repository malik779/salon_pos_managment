namespace StaffService.Domain.Entities;

public sealed class StaffMember
{
    public StaffMember(Guid id, string fullName, Guid defaultBranchId, string role)
    {
        Id = id;
        FullName = fullName;
        DefaultBranchId = defaultBranchId;
        Role = role;
    }

    public Guid Id { get; }
    public string FullName { get; }
    public Guid DefaultBranchId { get; }
    public string Role { get; }
}
