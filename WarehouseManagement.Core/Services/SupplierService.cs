using System.Collections.ObjectModel;
using WarehouseData.Models;
using WarehouseManagement.Core.DataStorage;
using WarehouseManagement.Core.Interfaces;

namespace WarehouseManagement.Core.Services;

public class SupplierService : ISupplierService
{
    public ObservableCollection<Supplier> GetAll() => InMemoryStorage.Suppliers;

    public Supplier? GetById(int id) =>
        InMemoryStorage.Suppliers.FirstOrDefault(s => s.Id == id);

    public Supplier Add(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Наименование поставщика не может быть пустым.");

        var sup = new Supplier { Name = name.Trim() };
        InMemoryStorage.Suppliers.Add(sup);
        return sup;
    }

    public bool Update(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Наименование поставщика не может быть пустым.");

        var sup = GetById(id);
        if (sup == null) return false;

        sup.Name = name.Trim();
        return true;
    }

    public bool Delete(int id)
    {
        var sup = GetById(id);
        if (sup == null) return false;

        InMemoryStorage.Suppliers.Remove(sup);
        return true;
    }
}
