using BuildingBlocks.Domain.Abstractions;

namespace Catalog.Domain.Services;

public sealed class ServiceItem : AuditableEntity, IAggregateRoot
{
    private ServiceItem()
    {
    }

    private ServiceItem(Guid id, string name, int durationMinutes, decimal basePrice, string category)
    {
        Id = id;
        Name = name;
        DurationMinutes = durationMinutes;
        BasePrice = basePrice;
        Category = category;
        IsActive = true;
    }

    public string Name { get; private set; } = default!;
    public int DurationMinutes { get; private set; }
        = 0;
    public decimal BasePrice { get; private set; }
        = 0;
    public string Category { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;

    public static ServiceItem Create(string name, int durationMinutes, decimal basePrice, string category)
        => new(Guid.NewGuid(), name, durationMinutes, basePrice, category);
}
