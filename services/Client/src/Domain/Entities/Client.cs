namespace ClientService.Domain.Entities;

public sealed class Client
{
    private Client()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Phone = string.Empty;
    }

    public Client(Guid id, string firstName, string lastName, string phone, string? email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = email;
    }

    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Phone { get; private set; }
    public string? Email { get; private set; }
}

public sealed record ClientConsent(string Version, DateTime CapturedAtUtc);
