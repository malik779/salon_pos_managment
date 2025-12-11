namespace StaffService.Domain.Entities;

public sealed class StaffMember
{
    private StaffMember()
    {
        FullName = string.Empty;
        Role = string.Empty;
    }

    public StaffMember(Guid id, string fullName, Guid defaultBranchId, string role)
    {
        Id = id;
        FullName = fullName;
        DefaultBranchId = defaultBranchId;
        Role = role;
    }

    public Guid Id { get; private set; }
    public string FullName { get; private set; }
    public Guid DefaultBranchId { get; private set; }
    public string Role { get; private set; }
}
