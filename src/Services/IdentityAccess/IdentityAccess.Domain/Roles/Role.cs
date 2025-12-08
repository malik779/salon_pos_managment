using BuildingBlocks.Domain.Abstractions;

namespace IdentityAccess.Domain.Roles;

public sealed class Role : AuditableEntity, IAggregateRoot
{
    private readonly HashSet<string> _permissions = new(StringComparer.OrdinalIgnoreCase);

    private Role()
    {
    }

    private Role(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Name { get; private set; } = default!;
    public IReadOnlyCollection<string> Permissions => _permissions.ToList().AsReadOnly();

    public static Role Create(string name, IEnumerable<string> permissions)
    {
        var role = new Role(Guid.NewGuid(), name);
        foreach (var permission in permissions)
        {
            role._permissions.Add(permission);
        }

        return role;
    }
}
