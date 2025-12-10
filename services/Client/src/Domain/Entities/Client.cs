namespace ClientService.Domain.Entities;

public sealed class Client
{
    public Client(Guid id, string firstName, string lastName, string phone, string? email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = email;
    }

    public Guid Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Phone { get; }
    public string? Email { get; }
}

public sealed record ClientConsent(string Version, DateTime CapturedAtUtc);
