namespace IdentityService.Domain;

public class UserAccount
{
    private UserAccount()
    {
        Email = string.Empty;
        FullName = string.Empty;
        Role = string.Empty;
    }

    public UserAccount(Guid id, string email, string fullName, Guid branchId, string role)
    {
        Id = id;
        Email = email;
        FullName = fullName;
        BranchId = branchId;
        Role = role;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string FullName { get; private set; }
    public Guid BranchId { get; private set; }
    public string Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
