using System.Collections.ObjectModel;

namespace WarehouseData.Models;

public class Organization
{
    private static int _counter = 0;

    public int OrgId { get; private set; }
    public string OrgName { get; set; } = string.Empty;
    public ObservableCollection<Warehouse> Warehouses { get; set; } = new();

    public Organization(string name)
    {
        OrgId = ++_counter;
        OrgName = name;
    }

    public static void ResetCounter() => _counter = 0;
}
