using System.Windows;
using WarehouseManagement.Core.Seed;

namespace WarehouseManagement.UI;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DataSeeder.Seed();
    }
}
