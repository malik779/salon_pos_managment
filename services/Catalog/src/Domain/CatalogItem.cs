namespace CatalogService.Domain;

public class CatalogItem
{
    private CatalogItem()
    {
        Name = string.Empty;
        Type = string.Empty;
    }

    public CatalogItem(Guid id, string name, string type, decimal price, int durationMinutes)
    {
        Id = id;
        Name = name;
        Type = type;
        Price = price;
        DurationMinutes = durationMinutes;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Type { get; private set; }
    public decimal Price { get; private set; }
    public int DurationMinutes { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void Update(string name, string type, decimal price, int durationMinutes)
    {
        Name = name;
        Type = type;
        Price = price;
        DurationMinutes = durationMinutes;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
