namespace IdentityService.Domain;

public class UserAccount
{
    private UserAccount()
    {
        Email = string.Empty;
        FullName = string.Empty;
        Role = string.Empty;
        PasswordHash = string.Empty;
    }

    public UserAccount(Guid id, string email, string fullName, Guid branchId, string role, string passwordHash)
    {
        Id = id;
        Email = email;
        FullName = fullName;
        BranchId = branchId;
        Role = role;
        PasswordHash = passwordHash;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string FullName { get; private set; }
    public Guid BranchId { get; private set; }
    public string Role { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? PasswordResetTokenExpiry { get; private set; }
    public string? PasswordResetToken { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiry { get; private set; }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    
    public void SetPasswordResetToken(string token, DateTime expiry)
    {
        PasswordResetToken = token;
        PasswordResetTokenExpiry = expiry;
    }
    
    public void ClearPasswordResetToken()
    {
        PasswordResetToken = null;
        PasswordResetTokenExpiry = null;
    }
    
    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        ClearPasswordResetToken();
    }
    
    public void SetRefreshToken(string token, DateTime expiry)
    {
        RefreshToken = token;
        RefreshTokenExpiry = expiry;
    }
    
    public void ClearRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiry = null;
    }
}
