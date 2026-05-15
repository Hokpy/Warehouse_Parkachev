using System.Collections.ObjectModel;
using WarehouseData.Models;
using WarehouseManagement.Core.DataStorage;
using WarehouseManagement.Core.Interfaces;

namespace WarehouseManagement.Core.Services;

public class CategoryService : ICategoryService
{
    public ObservableCollection<Category> GetAll() => InMemoryStorage.Categories;

    public Category? GetById(int id) =>
        InMemoryStorage.Categories.FirstOrDefault(c => c.Id == id);

    public Category Add(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Наименование категории не может быть пустым.");

        var cat = new Category { Name = name.Trim() };
        InMemoryStorage.Categories.Add(cat);
        return cat;
    }

    public bool Update(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Наименование категории не может быть пустым.");

        var cat = GetById(id);
        if (cat == null) return false;

        cat.Name = name.Trim();
        return true;
    }

    public bool Delete(int id)
    {
        var cat = GetById(id);
        if (cat == null) return false;

        InMemoryStorage.Categories.Remove(cat);
        return true;
    }
}
