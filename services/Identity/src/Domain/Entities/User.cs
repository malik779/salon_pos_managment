namespace IdentityService.Domain.Entities;

public sealed class User
{
    private User()
    {
        Email = string.Empty;
        FullName = string.Empty;
        Roles = Array.Empty<string>();
    }

    public User(Guid id, string email, string fullName, string[] roles)
    {
        Id = id;
        Email = email;
        FullName = fullName;
        Roles = roles;
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string FullName { get; private set; }
    public string[] Roles { get; private set; }
}
