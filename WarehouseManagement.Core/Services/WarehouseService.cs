using System.Collections.ObjectModel;
using WarehouseData.Models;
using WarehouseManagement.Core.DataStorage;
using WarehouseManagement.Core.Interfaces;

namespace WarehouseManagement.Core.Services;

public class WarehouseService : IWarehouseService
{
    /// <summary>
    /// Returns a snapshot collection for the given org. UI reloads it on demand.
    /// </summary>
    public ObservableCollection<Warehouse> GetByOrganization(int orgId) =>
        new(InMemoryStorage.Warehouses.Where(w => w.OrgId == orgId));

    public Warehouse? GetById(int id) =>
        InMemoryStorage.Warehouses.FirstOrDefault(w => w.WhId == id);

    public Warehouse Add(string name, string address, int orgId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Наименование склада не может быть пустым.", nameof(name));
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Адрес склада не может быть пустым.", nameof(address));

        var org = InMemoryStorage.Organizations.FirstOrDefault(o => o.OrgId == orgId)
            ?? throw new ArgumentException("Организация не найдена.", nameof(orgId));

        var warehouse = new Warehouse(name.Trim(), address.Trim(), orgId);
        InMemoryStorage.Warehouses.Add(warehouse);
        org.Warehouses.Add(warehouse);
        return warehouse;
    }

    public bool Update(int id, string name, string address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Наименование склада не может быть пустым.", nameof(name));
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Адрес склада не может быть пустым.", nameof(address));

        var wh = GetById(id);
        if (wh == null) return false;

        wh.WhName = name.Trim();
        wh.WhAddress = address.Trim();
        return true;
    }

    public bool Delete(int id)
    {
        var wh = GetById(id);
        if (wh == null) return false;

        // BUG FIX: was checking both Products global list AND wh.products — redundant and error-prone.
        // Only wh.Products is the source of truth for what's on this warehouse.
        if (wh.Products.Count > 0) return false;

        var org = InMemoryStorage.Organizations.FirstOrDefault(o => o.OrgId == wh.OrgId);
        org?.Warehouses.Remove(wh);
        InMemoryStorage.Warehouses.Remove(wh);
        return true;
    }
}
