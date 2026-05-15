using System.Collections.ObjectModel;
using WarehouseData.Models;

namespace WarehouseManagement.Core.Interfaces;

public interface IOrganizationService
{
    ObservableCollection<Organization> GetAll();
    Organization? GetById(int id);
    Organization Add(string name);
    bool Update(int id, string name);
    bool Delete(int id);
}

public interface IWarehouseService
{
    ObservableCollection<Warehouse> GetByOrganization(int orgId);
    Warehouse? GetById(int id);
    Warehouse Add(string name, string address, int orgId);
    bool Update(int id, string name, string address);
    bool Delete(int id);
}

public interface IProductService
{
    List<Product> GetByWarehouse(int warehouseId);
    Product? GetByArticle(string article);
    Product Add(Product product, int warehouseId);
    bool Update(Product product);
    bool Delete(string article);
    List<Product> ImportFromCsv(string csv, int warehouseId);
}

public interface ICategoryService
{
    ObservableCollection<Category> GetAll();
    Category? GetById(int id);
    Category Add(string name);
    bool Update(int id, string name);
    bool Delete(int id);
}

public interface IManufacturerService
{
    ObservableCollection<Manufacturer> GetAll();
    Manufacturer? GetById(int id);
    Manufacturer Add(string name);
    bool Update(int id, string name);
    bool Delete(int id);
}

public interface ISupplierService
{
    ObservableCollection<Supplier> GetAll();
    Supplier? GetById(int id);
    Supplier Add(string name);
    bool Update(int id, string name);
    bool Delete(int id);
}
