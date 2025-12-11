namespace ClientService.Domain;

public class ClientProfile
{
    private ClientProfile()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Phone = string.Empty;
        Email = string.Empty;
    }

    public ClientProfile(Guid id, string firstName, string lastName, string phone, string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = email;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Phone { get; private set; }
    public string Email { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
}
