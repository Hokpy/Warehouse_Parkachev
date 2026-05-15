using WarehouseData.Models;
using WarehouseManagement.Core.DataStorage;

namespace WarehouseManagement.Tests;

/// <summary>
/// Resets all InMemoryStorage collections AND static ID counters before AND after each test.
/// 
/// xUnit creates ONE instance of each test class and runs all test methods on it.
/// The constructor runs once per class, not per test — so cleanup must also happen
/// in Dispose() which xUnit calls after every individual test method.
/// </summary>
public abstract class TestBase : IDisposable
{
    protected TestBase() => Reset();
    public void Dispose() => Reset();

    private static void Reset()
    {
        InMemoryStorage.Organizations.Clear();
        InMemoryStorage.Warehouses.Clear();
        InMemoryStorage.Products.Clear();
        InMemoryStorage.Categories.Clear();
        InMemoryStorage.Manufacturers.Clear();
        InMemoryStorage.Suppliers.Clear();

        Organization.ResetCounter();
        Warehouse.ResetCounter();
        // Product has no counter — Article is its identity, skip
    }
}