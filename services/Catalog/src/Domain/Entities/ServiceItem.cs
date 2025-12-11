namespace CatalogService.Domain.Entities;

public sealed class ServiceItem
{
    private ServiceItem()
    {
        Name = string.Empty;
    }

    public ServiceItem(Guid id, string name, int durationMinutes, decimal basePrice)
    {
        Id = id;
        Name = name;
        DurationMinutes = durationMinutes;
        BasePrice = basePrice;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public int DurationMinutes { get; private set; }
    public decimal BasePrice { get; private set; }
}
