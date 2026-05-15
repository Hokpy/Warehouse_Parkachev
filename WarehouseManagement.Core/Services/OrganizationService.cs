using System.Collections.ObjectModel;
using WarehouseData.Models;
using WarehouseManagement.Core.DataStorage;
using WarehouseManagement.Core.Interfaces;

namespace WarehouseManagement.Core.Services;

public class OrganizationService : IOrganizationService
{
    public ObservableCollection<Organization> GetAll() => InMemoryStorage.Organizations;

    public Organization? GetById(int id) =>
        InMemoryStorage.Organizations.FirstOrDefault(o => o.OrgId == id);

    public Organization Add(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Наименование организации не может быть пустым.", nameof(name));

        var org = new Organization(name.Trim());
        InMemoryStorage.Organizations.Add(org);
        return org;
    }

    public bool Update(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Наименование организации не может быть пустым.", nameof(name));

        var org = GetById(id);
        if (org == null) return false;

        org.OrgName = name.Trim();
        return true;
    }

    public bool Delete(int id)
    {
        var org = GetById(id);
        if (org == null) return false;

        if (InMemoryStorage.Warehouses.Any(w => w.OrgId == id))
            return false;

        InMemoryStorage.Organizations.Remove(org);
        return true;
    }
}
