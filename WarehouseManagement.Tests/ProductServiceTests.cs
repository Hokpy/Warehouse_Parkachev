using WarehouseData.Models;
using WarehouseManagement.Core.Services;
using Xunit;

namespace WarehouseManagement.Tests;

public class ProductServiceTests : TestBase
{
    private readonly OrganizationService _orgService = new();
    private readonly WarehouseService _whService = new();
    private readonly ProductService _service = new();

    private (Warehouse wh, Product product) CreateWarehouseAndProduct(
        string article = "P001", string name = "Тестовый товар")
    {
        var org = _orgService.Add("Org");
        var wh = _whService.Add("Склад", "Адрес", org.OrgId);
        var product = new Product
        {
            Article = article, Name = name, Unit = "шт",
            Price = 100m, StockQuantity = 10, DiscountPercent = 0
        };
        _service.Add(product, wh.WhId);
        return (wh, product);
    }

    [Fact]
    public void Add_ValidProduct_AddsToStorage()
    {
        CreateWarehouseAndProduct();
        var found = _service.GetByArticle("P001");
        Assert.NotNull(found);
        Assert.Equal("Тестовый товар", found!.Name);
    }

    [Fact]
    public void Add_ValidProduct_AppearsInWarehouseProducts()
    {
        var (wh, product) = CreateWarehouseAndProduct();
        Assert.Contains(product, wh.Products);
    }

    [Fact]
    public void Add_DuplicateArticle_ThrowsInvalidOperation()
    {
        var (wh, _) = CreateWarehouseAndProduct();
        var dup = new Product { Article = "P001", Name = "Другой", Unit = "шт", Price = 50, StockQuantity = 1, DiscountPercent = 0 };
        Assert.Throws<InvalidOperationException>(() => _service.Add(dup, wh.WhId));
    }

    [Fact]
    public void Add_NegativePrice_ThrowsArgumentException()
    {
        var org = _orgService.Add("Org");
        var wh = _whService.Add("Склад", "Адрес", org.OrgId);
        var bad = new Product { Article = "PNEG", Name = "Плохой", Unit = "шт", Price = -1, StockQuantity = 1, DiscountPercent = 0 };
        Assert.Throws<ArgumentException>(() => _service.Add(bad, wh.WhId));
    }

    [Fact]
    public void Add_NegativeStock_ThrowsArgumentException()
    {
        var org = _orgService.Add("Org");
        var wh = _whService.Add("Склад", "Адрес", org.OrgId);
        var bad = new Product { Article = "PSTK", Name = "Плохой", Unit = "шт", Price = 10, StockQuantity = -1, DiscountPercent = 0 };
        Assert.Throws<ArgumentException>(() => _service.Add(bad, wh.WhId));
    }

    [Fact]
    public void Add_DiscountOutOfRange_ThrowsArgumentException()
    {
        var org = _orgService.Add("Org");
        var wh = _whService.Add("Склад", "Адрес", org.OrgId);
        var bad = new Product { Article = "PDSC", Name = "Плохой", Unit = "шт", Price = 100, StockQuantity = 1, DiscountPercent = 150 };
        Assert.Throws<ArgumentException>(() => _service.Add(bad, wh.WhId));
    }

    [Fact]
    public void Add_InvalidWarehouseId_ThrowsArgumentException()
    {
        var product = new Product { Article = "PINV", Name = "Товар", Unit = "шт", Price = 10, StockQuantity = 1, DiscountPercent = 0 };
        Assert.Throws<ArgumentException>(() => _service.Add(product, 99999));
    }

    [Fact]
    public void Delete_RemovesFromStorageAndWarehouse()
    {
        var (wh, _) = CreateWarehouseAndProduct();
        var result = _service.Delete("P001");
        Assert.True(result);
        Assert.Null(_service.GetByArticle("P001"));
        Assert.Empty(wh.Products);
    }

    [Fact]
    public void Delete_NonExistent_ReturnsFalse()
    {
        var result = _service.Delete("NOTEXIST");
        Assert.False(result);
    }

    [Fact]
    public void Update_ChangesFields()
    {
        CreateWarehouseAndProduct();
        var updated = new Product
        {
            Article = "P001", Name = "Обновлённый", Unit = "кг",
            Price = 200m, StockQuantity = 20, DiscountPercent = 10
        };
        var result = _service.Update(updated);
        Assert.True(result);
        var found = _service.GetByArticle("P001");
        Assert.Equal("Обновлённый", found!.Name);
        Assert.Equal(200m, found.Price);
        Assert.Equal(20, found.StockQuantity);
    }

    [Fact]
    public void Update_NonExistent_ReturnsFalse()
    {
        var ghost = new Product { Article = "GHOST", Name = "Ghost", Unit = "шт", Price = 1, StockQuantity = 1, DiscountPercent = 0 };
        var result = _service.Update(ghost);
        Assert.False(result);
    }

    [Fact]
    public void GetByWarehouse_ReturnsCorrectProducts()
    {
        var org = _orgService.Add("Org");
        var wh1 = _whService.Add("Склад1", "Адрес1", org.OrgId);
        var wh2 = _whService.Add("Склад2", "Адрес2", org.OrgId);

        _service.Add(new Product { Article = "A1", Name = "T1", Unit = "шт", Price = 10, StockQuantity = 1, DiscountPercent = 0 }, wh1.WhId);
        _service.Add(new Product { Article = "A2", Name = "T2", Unit = "шт", Price = 20, StockQuantity = 1, DiscountPercent = 0 }, wh2.WhId);

        var wh1Products = _service.GetByWarehouse(wh1.WhId);
        Assert.Single(wh1Products);
        Assert.Equal("A1", wh1Products[0].Article);
    }

    [Fact]
    public void ImportFromCsv_ValidLines_ImportsProducts()
    {
        var org = _orgService.Add("Org");
        var wh = _whService.Add("Склад", "Адрес", org.OrgId);
        // Use InvariantCulture decimal separator (dot)
        var csv = "CSV001;Импорт товар;шт;500.00;0;0;0;5.0;100\nCSV002;Второй товар;кг;250.00;0;0;0;0;50";

        var result = _service.ImportFromCsv(csv, wh.WhId);

        Assert.Equal(2, result.Count);
        Assert.NotNull(_service.GetByArticle("CSV001"));
        Assert.NotNull(_service.GetByArticle("CSV002"));
    }

    [Fact]
    public void ImportFromCsv_DuplicateArticle_SkipsExisting()
    {
        var org = _orgService.Add("Org");
        var wh = _whService.Add("Склад", "Адрес", org.OrgId);
        _service.Add(new Product { Article = "DUP001", Name = "Существующий", Unit = "шт", Price = 100, StockQuantity = 1, DiscountPercent = 0 }, wh.WhId);

        var csv = "DUP001;Новый;шт;200;0;0;0;0;5";
        var result = _service.ImportFromCsv(csv, wh.WhId);

        Assert.Empty(result);
        // Original should be unchanged
        Assert.Equal("Существующий", _service.GetByArticle("DUP001")!.Name);  
        }

    [Fact]
    public void ImportFromCsv_InvalidLines_SkippedGracefully()
    {
        var org = _orgService.Add("Org");
        var wh = _whService.Add("Склад", "Адрес", org.OrgId);
        var csv = "BADLINE\n;;\nGOOD001;Нормальный;шт;100;0;0;0;0;10";

        var result = _service.ImportFromCsv(csv, wh.WhId);

        Assert.Single(result);
    }
}
