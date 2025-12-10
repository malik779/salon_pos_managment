namespace CatalogService.Application.Catalog.Models;

public sealed record ServiceDto(Guid Id, string Name, int DurationMinutes, decimal BasePrice);
public sealed record ProductDto(Guid Id, string Sku, string Name, decimal Price, int InventoryQty);
