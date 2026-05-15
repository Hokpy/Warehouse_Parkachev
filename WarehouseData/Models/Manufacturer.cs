namespace WarehouseData.Models;

public class Manufacturer
{
    private static int _counter = 0;
    public int Id { get; private set; }
    public string Name { get; set; } = string.Empty;

    public Manufacturer() { Id = ++_counter; }
    public static void ResetCounter() => _counter = 0;
}
