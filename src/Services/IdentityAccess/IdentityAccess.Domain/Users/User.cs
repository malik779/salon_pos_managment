using BuildingBlocks.Domain.Abstractions;

namespace IdentityAccess.Domain.Users;

public sealed class User : AuditableEntity, IAggregateRoot
{
    private readonly List<Guid> _roles = new();

    private User()
    {
    }

    private User(Guid id, string email, string name, Guid defaultBranchId, bool isActive)
    {
        Id = id;
        Email = email;
        Name = name;
        DefaultBranchId = defaultBranchId;
        IsActive = isActive;
    }

    public string Email { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public Guid DefaultBranchId { get; private set; }
        = Guid.Empty;
    public bool IsActive { get; private set; } = true;

    public IReadOnlyCollection<Guid> Roles => _roles.AsReadOnly();

    public static User Create(string email, string name, Guid defaultBranchId)
        => new(Guid.NewGuid(), email, name, defaultBranchId, true);

    public void AssignRole(Guid roleId)
    {
        if (!_roles.Contains(roleId))
        {
            _roles.Add(roleId);
        }
    }

    public void Deactivate() => IsActive = false;
}
