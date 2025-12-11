namespace CatalogService.Domain.Entities;

public sealed class ProductItem
{
    private ProductItem()
    {
        Sku = string.Empty;
        Name = string.Empty;
    }

    public ProductItem(Guid id, string sku, string name, decimal price, int inventoryQty)
    {
        Id = id;
        Sku = sku;
        Name = name;
        Price = price;
        InventoryQty = inventoryQty;
    }

    public Guid Id { get; private set; }
    public string Sku { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public int InventoryQty { get; private set; }

    public void AdjustInventory(int delta)
    {
        InventoryQty += delta;
    }
}
