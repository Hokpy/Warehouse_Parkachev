using System.Collections.ObjectModel;
using WarehouseData.Models;

namespace WarehouseManagement.Core.DataStorage;

public static class InMemoryStorage
{
    public static ObservableCollection<Organization> Organizations { get; } = new();
    public static ObservableCollection<Warehouse> Warehouses { get; } = new();
    public static ObservableCollection<Product> Products { get; } = new();
    public static ObservableCollection<Category> Categories { get; } = new();
    public static ObservableCollection<Manufacturer> Manufacturers { get; } = new();
    public static ObservableCollection<Supplier> Suppliers { get; } = new();
}
