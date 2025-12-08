using BuildingBlocks.Domain.Abstractions;

namespace BranchConfiguration.Domain.Branches;

public sealed class Branch : AuditableEntity, IAggregateRoot
{
    private Branch()
    {
    }

    private Branch(Guid id, string name, string timezone, string address, decimal defaultTaxRate, TimeSpan openTimeUtc, TimeSpan closeTimeUtc)
    {
        Id = id;
        Name = name;
        Timezone = timezone;
        Address = address;
        DefaultTaxRate = defaultTaxRate;
        OpenTimeUtc = openTimeUtc;
        CloseTimeUtc = closeTimeUtc;
    }

    public string Name { get; private set; } = default!;
    public string Timezone { get; private set; } = default!;
    public string Address { get; private set; } = default!;
    public decimal DefaultTaxRate { get; private set; }
        = 0;
    public TimeSpan OpenTimeUtc { get; private set; }
        = TimeSpan.Zero;
    public TimeSpan CloseTimeUtc { get; private set; }
        = TimeSpan.Zero;

    public static Branch Create(string name, string timezone, string address, decimal defaultTaxRate, TimeSpan openTimeUtc, TimeSpan closeTimeUtc)
        => new(Guid.NewGuid(), name, timezone, address, defaultTaxRate, openTimeUtc, closeTimeUtc);
}
