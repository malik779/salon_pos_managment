namespace BranchService.Domain;

public class Branch
{
    private Branch()
    {
        Name = string.Empty;
        Timezone = string.Empty;
        Address = string.Empty;
    }

    public Branch(Guid id, string name, string timezone, string address)
    {
        Id = id;
        Name = name;
        Timezone = timezone;
        Address = address;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Timezone { get; private set; }
    public string Address { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public void UpdateDetails(string name, string timezone, string address)
    {
        Name = name;
        Timezone = timezone;
        Address = address;
    }
}
