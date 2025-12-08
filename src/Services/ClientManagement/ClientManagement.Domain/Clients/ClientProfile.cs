using BuildingBlocks.Domain.Abstractions;

namespace ClientManagement.Domain.Clients;

public sealed class ClientProfile : AuditableEntity, IAggregateRoot
{
    private ClientProfile()
    {
    }

    private ClientProfile(Guid id, string name, string email, string phone)
    {
        Id = id;
        Name = name;
        Email = email;
        Phone = phone;
    }

    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public DateOnly? DateOfBirth { get; private set; }
        = null;

    public static ClientProfile Create(string name, string email, string phone)
        => new(Guid.NewGuid(), name, email, phone);
}
