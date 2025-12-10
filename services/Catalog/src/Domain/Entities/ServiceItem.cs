namespace CatalogService.Domain.Entities;

public sealed class ServiceItem
{
    public ServiceItem(Guid id, string name, int durationMinutes, decimal basePrice)
    {
        Id = id;
        Name = name;
        DurationMinutes = durationMinutes;
        BasePrice = basePrice;
    }

    public Guid Id { get; }
    public string Name { get; }
    public int DurationMinutes { get; }
    public decimal BasePrice { get; }
}
