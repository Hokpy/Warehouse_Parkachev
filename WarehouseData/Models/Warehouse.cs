namespace WarehouseData.Models;

public class Warehouse
{
    private static int _counter = 0;

    public int WhId { get; private set; }
    public string WhName { get; set; } = string.Empty;
    public string WhAddress { get; set; } = string.Empty;
    public int OrgId { get; set; }
    public List<Product> Products { get; set; } = new();

    public Warehouse(string name, string address, int orgId)
    {
        WhId = ++_counter;
        WhName = name;
        WhAddress = address;
        OrgId = orgId;
    }

    public static void ResetCounter() => _counter = 0;
}
