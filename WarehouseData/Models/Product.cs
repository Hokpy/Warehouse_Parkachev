namespace WarehouseData.Models;

public class Product
{
    public string Article { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public decimal DiscountPercent { get; set; }
    public int CategoryId { get; set; }
    public int ManufacturerId { get; set; }
    public int SupplierId { get; set; }
    public string? Description { get; set; }
    public string? PhotoPath { get; set; }
    public string SupplierName { get; set; } = string.Empty;

    // Navigation properties
    public Category? Category { get; set; }
    public Manufacturer? Manufacturer { get; set; }
    public Supplier? Supplier { get; set; }
}
