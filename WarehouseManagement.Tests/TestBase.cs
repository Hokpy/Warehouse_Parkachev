using WarehouseData.Models;
using WarehouseManagement.Core.DataStorage;

namespace WarehouseManagement.Tests;

/// <summary>
/// Resets all InMemoryStorage collections AND static ID counters before each test.
/// Without counter resets, tests that rely on specific IDs can fail depending on run order.
/// </summary>
public abstract class TestBase : IDisposable
{
    protected TestBase()
    {
        // Clear storage
        InMemoryStorage.Organizations.Clear();
        InMemoryStorage.Warehouses.Clear();
        InMemoryStorage.Products.Clear();
        InMemoryStorage.Categories.Clear();
        InMemoryStorage.Manufacturers.Clear();
        InMemoryStorage.Suppliers.Clear();

        // Reset static counters — critical for predictable IDs across tests
        Organization.ResetCounter();
        Warehouse.ResetCounter();
        Category.ResetCounter();
        Manufacturer.ResetCounter();
        Supplier.ResetCounter();
    }

    public void Dispose() { }
}
