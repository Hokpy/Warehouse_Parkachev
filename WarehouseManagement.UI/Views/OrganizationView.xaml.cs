using System.Windows.Controls;
using WarehouseManagement.UI.ViewModels;

namespace WarehouseManagement.UI.Views;

public partial class OrganizationView : UserControl
{
    public OrganizationView()
    {
        InitializeComponent();
    }

    private void OrgList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (DataContext is OrganizationViewModel vm)
            vm.SelectOrgForEditCommand.Execute(null);
    }

    private void WhList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (DataContext is OrganizationViewModel vm)
            vm.SelectWhForEditCommand.Execute(null);
    }
}
