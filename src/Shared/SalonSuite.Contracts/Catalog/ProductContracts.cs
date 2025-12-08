namespace SalonSuite.Contracts.Catalog;

public sealed record ServiceDto(Guid Id, string Name, int DurationMinutes, decimal BasePrice, string Category);

public sealed record ProductDto(Guid Id, string Sku, string Name, decimal Price, int InventoryQty);
