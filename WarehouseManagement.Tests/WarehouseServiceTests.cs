using WarehouseManagement.Core.Services;
using Xunit;

namespace WarehouseManagement.Tests;

public class WarehouseServiceTests : TestBase
{
    private readonly OrganizationService _orgService = new();
    private readonly WarehouseService _service = new();

    [Fact]
    public void Add_AssignsCorrectOrgId()
    {
        var org = _orgService.Add("Org");
        var wh = _service.Add("Склад", "Адрес 1", org.OrgId);
        Assert.Equal(org.OrgId, wh.OrgId);
    }

    [Fact]
    public void Add_EmptyName_ThrowsArgumentException()
    {
        var org = _orgService.Add("Org");
        Assert.Throws<ArgumentException>(() => _service.Add("", "Адрес", org.OrgId));
    }

    [Fact]
    public void Add_EmptyAddress_ThrowsArgumentException()
    {
        var org = _orgService.Add("Org");
        Assert.Throws<ArgumentException>(() => _service.Add("Склад", "", org.OrgId));
    }

    [Fact]
    public void Add_InvalidOrgId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _service.Add("Склад", "Адрес", 99999));
    }

    [Fact]
    public void Add_AlsoAppearsInOrgWarehouses()
    {
        var org = _orgService.Add("Org");
        var wh = _service.Add("Склад", "Адрес", org.OrgId);
        Assert.Contains(wh, org.Warehouses);
    }

    [Fact]
    public void GetByOrganization_ReturnsOnlyMatchingWarehouses()
    {
        var org1 = _orgService.Add("Org1");
        var org2 = _orgService.Add("Org2");
        _service.Add("Склад1", "Адрес1", org1.OrgId);
        _service.Add("Склад2", "Адрес2", org1.OrgId);
        _service.Add("Склад3", "Адрес3", org2.OrgId);

        var result = _service.GetByOrganization(org1.OrgId);

        Assert.Equal(2, result.Count);
        Assert.All(result, w => Assert.Equal(org1.OrgId, w.OrgId));
    }

    [Fact]
    public void Delete_WithProducts_ReturnsFalse()
    {
        var org = _orgService.Add("Org");
        var wh = _service.Add("Склад", "Адрес", org.OrgId);
        var prodService = new ProductService();
        var product = new WarehouseData.Models.Product
        {
            Article = "TST001", Name = "Товар", Unit = "шт",
            Price = 100, StockQuantity = 5, DiscountPercent = 0
        };
        prodService.Add(product, wh.WhId);

        var result = _service.Delete(wh.WhId);

        Assert.False(result);
    }

    [Fact]
    public void Delete_EmptyWarehouse_ReturnsTrue()
    {
        var org = _orgService.Add("Org");
        var wh = _service.Add("Пустой склад", "Адрес", org.OrgId);
        var result = _service.Delete(wh.WhId);
        Assert.True(result);
    }

    [Fact]
    public void Delete_RemovesFromOrganizationWarehouses()
    {
        var org = _orgService.Add("Org");
        var wh = _service.Add("Склад", "Адрес", org.OrgId);
        _service.Delete(wh.WhId);
        Assert.DoesNotContain(wh, org.Warehouses);
    }

    [Fact]
    public void Delete_NonExistent_ReturnsFalse()
    {
        var result = _service.Delete(99999);
        Assert.False(result);
    }

    [Fact]
    public void Update_ChangesNameAndAddress()
    {
        var org = _orgService.Add("Org");
        var wh = _service.Add("Старое", "Старый адрес", org.OrgId);
        var result = _service.Update(wh.WhId, "Новое", "Новый адрес");
        Assert.True(result);
        Assert.Equal("Новое", wh.WhName);
        Assert.Equal("Новый адрес", wh.WhAddress);
    }

    [Fact]
    public void Update_NonExistent_ReturnsFalse()
    {
        var result = _service.Update(99999, "Имя", "Адрес");
        Assert.False(result);
    }
}
