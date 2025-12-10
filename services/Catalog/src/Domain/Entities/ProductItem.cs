namespace CatalogService.Domain.Entities;

public sealed class ProductItem
{
    public ProductItem(Guid id, string sku, string name, decimal price, int inventoryQty)
    {
        Id = id;
        Sku = sku;
        Name = name;
        Price = price;
        InventoryQty = inventoryQty;
    }

    public Guid Id { get; }
    public string Sku { get; }
    public string Name { get; }
    public decimal Price { get; private set; }
    public int InventoryQty { get; private set; }

    public void AdjustInventory(int delta)
    {
        InventoryQty += delta;
    }
}
