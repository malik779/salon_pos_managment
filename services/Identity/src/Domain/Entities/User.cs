namespace IdentityService.Domain.Entities;

public sealed class User
{
    public User(Guid id, string email, string fullName, string[] roles)
    {
        Id = id;
        Email = email;
        FullName = fullName;
        Roles = roles;
    }

    public Guid Id { get; }
    public string Email { get; }
    public string FullName { get; }
    public string[] Roles { get; }
}
