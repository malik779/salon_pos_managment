namespace BranchService.Domain.Entities;

public sealed class Branch
{
    public Branch(Guid id, string name, string timezone, string address)
    {
        Id = id;
        Name = name;
        Timezone = timezone;
        Address = address;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Timezone { get; }
    public string Address { get; }
}
