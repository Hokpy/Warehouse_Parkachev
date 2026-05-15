using System.Collections.ObjectModel;
using WarehouseData.Models;
using WarehouseManagement.Core.DataStorage;
using WarehouseManagement.Core.Interfaces;

namespace WarehouseManagement.Core.Services;

public class ManufacturerService : IManufacturerService
{
    public ObservableCollection<Manufacturer> GetAll() => InMemoryStorage.Manufacturers;

    public Manufacturer? GetById(int id) =>
        InMemoryStorage.Manufacturers.FirstOrDefault(m => m.Id == id);

    public Manufacturer Add(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Наименование производителя не может быть пустым.");

        var man = new Manufacturer { Name = name.Trim() };
        InMemoryStorage.Manufacturers.Add(man);
        return man;
    }

    public bool Update(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Наименование производителя не может быть пустым.");

        var man = GetById(id);
        if (man == null) return false;

        man.Name = name.Trim();
        return true;
    }

    public bool Delete(int id)
    {
        var man = GetById(id);
        if (man == null) return false;

        InMemoryStorage.Manufacturers.Remove(man);
        return true;
    }
}
