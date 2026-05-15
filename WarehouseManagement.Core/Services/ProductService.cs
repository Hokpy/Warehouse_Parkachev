using WarehouseData.Models;
using WarehouseManagement.Core.DataStorage;
using WarehouseManagement.Core.Interfaces;

namespace WarehouseManagement.Core.Services;

public class ProductService : IProductService
{
    public List<Product> GetByWarehouse(int warehouseId)
    {
        var wh = InMemoryStorage.Warehouses.FirstOrDefault(w => w.WhId == warehouseId);
        return wh?.Products ?? new List<Product>();
    }

    public Product? GetByArticle(string article) =>
        InMemoryStorage.Products.FirstOrDefault(p =>
            p.Article.Equals(article, StringComparison.OrdinalIgnoreCase));

    public Product Add(Product product, int warehouseId)
    {
        ValidateProduct(product);

        if (GetByArticle(product.Article) != null)
            throw new InvalidOperationException($"Товар с артикулом '{product.Article}' уже существует.");

        var wh = InMemoryStorage.Warehouses.FirstOrDefault(w => w.WhId == warehouseId)
            ?? throw new ArgumentException("Склад не найден.");

        ResolveNavigationProperties(product);

        InMemoryStorage.Products.Add(product);
        wh.Products.Add(product);
        return product;
    }

    public bool Update(Product product)
    {
        ValidateProduct(product);

        var existing = GetByArticle(product.Article);
        if (existing == null) return false;

        existing.Name = product.Name;
        existing.Unit = product.Unit;
        existing.Price = product.Price;
        existing.StockQuantity = product.StockQuantity;
        existing.DiscountPercent = product.DiscountPercent;
        existing.CategoryId = product.CategoryId;
        existing.ManufacturerId = product.ManufacturerId;
        existing.SupplierId = product.SupplierId;
        existing.Description = product.Description;
        existing.PhotoPath = product.PhotoPath;

        ResolveNavigationProperties(existing);
        return true;
    }

    public bool Delete(string article)
    {
        var product = GetByArticle(article);
        if (product == null) return false;

        foreach (var wh in InMemoryStorage.Warehouses)
            wh.Products.Remove(product);

        InMemoryStorage.Products.Remove(product);
        return true;
    }

    public List<Product> ImportFromCsv(string csv, int warehouseId)
    {
        var result = new List<Product>();
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            // Skip header-like lines
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) continue;

            var parts = trimmed.Split(';');
            if (parts.Length < 9) continue;

            try
            {
                var product = new Product
                {
                    Article = parts[0].Trim(),
                    Name = parts[1].Trim(),
                    Unit = parts[2].Trim(),
                    // BUG FIX: use InvariantCulture to handle decimal separator consistently
                    Price = decimal.Parse(parts[3].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                    CategoryId = int.Parse(parts[4].Trim()),
                    ManufacturerId = int.Parse(parts[5].Trim()),
                    SupplierId = int.Parse(parts[6].Trim()),
                    DiscountPercent = decimal.Parse(parts[7].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                    StockQuantity = int.Parse(parts[8].Trim()),
                    Description = parts.Length > 9 ? parts[9].Trim() : null,
                    PhotoPath = parts.Length > 10 ? parts[10].Trim() : null,
                };

                if (GetByArticle(product.Article) == null)
                {
                    Add(product, warehouseId);
                    result.Add(product);
                }
            }
            catch
            {
                // Skip invalid lines silently; caller gets count of successfully imported rows
            }
        }

        return result;
    }

    private static void ResolveNavigationProperties(Product p)
    {
        p.Category = InMemoryStorage.Categories.FirstOrDefault(c => c.Id == p.CategoryId);
        p.Manufacturer = InMemoryStorage.Manufacturers.FirstOrDefault(m => m.Id == p.ManufacturerId);
        p.Supplier = InMemoryStorage.Suppliers.FirstOrDefault(s => s.Id == p.SupplierId);
        p.SupplierName = p.Supplier?.Name ?? string.Empty;
    }

    private static void ValidateProduct(Product p)
    {
        if (string.IsNullOrWhiteSpace(p.Article))
            throw new ArgumentException("Артикул не может быть пустым.");
        if (string.IsNullOrWhiteSpace(p.Name))
            throw new ArgumentException("Наименование товара не может быть пустым.");
        if (string.IsNullOrWhiteSpace(p.Unit))
            throw new ArgumentException("Единица измерения не может быть пустой.");
        if (p.Price < 0)
            throw new ArgumentException("Цена не может быть отрицательной.");
        if (p.StockQuantity < 0)
            throw new ArgumentException("Остаток не может быть отрицательным.");
        if (p.DiscountPercent < 0 || p.DiscountPercent > 100)
            throw new ArgumentException("Скидка должна быть от 0 до 100.");
    }
}
