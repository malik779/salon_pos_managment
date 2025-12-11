namespace StaffService.Domain;

public class StaffMember
{
    private StaffMember()
    {
        Role = string.Empty;
    }

    public StaffMember(Guid id, Guid userId, Guid defaultBranchId, string role)
    {
        Id = id;
        UserId = userId;
        DefaultBranchId = defaultBranchId;
        Role = role;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid DefaultBranchId { get; private set; }
    public string Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
}
