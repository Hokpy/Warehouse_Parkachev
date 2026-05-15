using WarehouseManagement.Core.Interfaces;
using WarehouseManagement.Core.Services;

namespace WarehouseManagement.UI.ViewModels;

public class MainViewModel : BaseViewModel
{
    private BaseViewModel _currentView = null!;

    public BaseViewModel CurrentView
    {
        get => _currentView;
        set => SetField(ref _currentView, value);
    }

    public OrganizationViewModel OrganizationVM { get; }
    public ReferenceViewModel ReferenceVM { get; }

    public RelayCommand ShowOrganizationsCommand { get; }
    public RelayCommand ShowReferencesCommand { get; }

    public MainViewModel()
    {
        IOrganizationService orgService = new OrganizationService();
        IWarehouseService whService = new WarehouseService();
        IProductService prodService = new ProductService();
        ICategoryService catService = new CategoryService();
        IManufacturerService manService = new ManufacturerService();
        ISupplierService supService = new SupplierService();

        OrganizationVM = new OrganizationViewModel(orgService, whService, prodService, catService, manService, supService);
        ReferenceVM = new ReferenceViewModel(catService, manService, supService);

        ShowOrganizationsCommand = new RelayCommand(() => CurrentView = OrganizationVM);
        ShowReferencesCommand = new RelayCommand(() => CurrentView = ReferenceVM);

        _currentView = OrganizationVM;
    }
}
