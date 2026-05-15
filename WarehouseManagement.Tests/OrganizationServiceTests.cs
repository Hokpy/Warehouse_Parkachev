using WarehouseManagement.Core.Services;
using Xunit;

namespace WarehouseManagement.Tests;

public class OrganizationServiceTests : TestBase
{
    private readonly OrganizationService _service = new();

    [Fact]
    public void Add_ValidName_CreatesOrganization()
    {
        var org = _service.Add("Тест ООО");
        Assert.NotNull(org);
        Assert.Equal("Тест ООО", org.OrgName);
        Assert.True(org.OrgId > 0);
        Assert.Single(_service.GetAll());
    }

    [Fact]
    public void Add_EmptyName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _service.Add(""));
        Assert.Throws<ArgumentException>(() => _service.Add("   "));
    }

    [Fact]
    public void Add_AutoIncrementsId()
    {
        var org1 = _service.Add("Org1");
        var org2 = _service.Add("Org2");
        Assert.NotEqual(org1.OrgId, org2.OrgId);
        Assert.True(org2.OrgId > org1.OrgId);
    }

    [Fact]
    public void Delete_WithWarehouses_ReturnsFalse()
    {
        var org = _service.Add("Org с складами");
        var whService = new WarehouseService();
        whService.Add("Склад", "Адрес", org.OrgId);

        var result = _service.Delete(org.OrgId);

        Assert.False(result);
        Assert.Single(_service.GetAll());
    }

    [Fact]
    public void Delete_ExistingWithoutWarehouses_ReturnsTrue()
    {
        var org = _service.Add("Org без складов");
        var result = _service.Delete(org.OrgId);
        Assert.True(result);
        Assert.Empty(_service.GetAll());
    }

    [Fact]
    public void Delete_NonExistent_ReturnsFalse()
    {
        var result = _service.Delete(99999);
        Assert.False(result);
    }

    [Fact]
    public void Update_ChangesName()
    {
        var org = _service.Add("Старое имя");
        var result = _service.Update(org.OrgId, "Новое имя");
        Assert.True(result);
        Assert.Equal("Новое имя", org.OrgName);
    }

    [Fact]
    public void Update_NonExistent_ReturnsFalse()
    {
        var result = _service.Update(99999, "Имя");
        Assert.False(result);
    }

    [Fact]
    public void Update_EmptyName_ThrowsArgumentException()
    {
        var org = _service.Add("Org");
        Assert.Throws<ArgumentException>(() => _service.Update(org.OrgId, ""));
    }

    [Fact]
    public void GetById_ReturnsCorrectOrganization()
    {
        var org = _service.Add("Найти меня");
        var found = _service.GetById(org.OrgId);
        Assert.NotNull(found);
        Assert.Equal(org.OrgId, found!.OrgId);
    }

    [Fact]
    public void GetById_NonExistent_ReturnsNull()
    {
        var found = _service.GetById(99999);
        Assert.Null(found);
    }
}
