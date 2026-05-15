using System.Collections.ObjectModel;
using System.Windows;
using WarehouseData.Models;
using WarehouseManagement.Core.Interfaces;

namespace WarehouseManagement.UI.ViewModels;

public class ReferenceViewModel : BaseViewModel
{
    private readonly ICategoryService _catService;
    private readonly IManufacturerService _manService;
    private readonly ISupplierService _supService;

    // Categories
    public ObservableCollection<Category> Categories { get; }
    private Category? _selectedCategory;
    public Category? SelectedCategory { get => _selectedCategory; set { SetField(ref _selectedCategory, value); if (value != null) CategoryName = value.Name; } }
    private string _categoryName = string.Empty;
    public string CategoryName { get => _categoryName; set => SetField(ref _categoryName, value); }

    // Manufacturers
    public ObservableCollection<Manufacturer> Manufacturers { get; }
    private Manufacturer? _selectedManufacturer;
    public Manufacturer? SelectedManufacturer { get => _selectedManufacturer; set { SetField(ref _selectedManufacturer, value); if (value != null) ManufacturerName = value.Name; } }
    private string _manufacturerName = string.Empty;
    public string ManufacturerName { get => _manufacturerName; set => SetField(ref _manufacturerName, value); }

    // Suppliers
    public ObservableCollection<Supplier> Suppliers { get; }
    private Supplier? _selectedSupplier;
    public Supplier? SelectedSupplier { get => _selectedSupplier; set { SetField(ref _selectedSupplier, value); if (value != null) SupplierName = value.Name; } }
    private string _supplierName = string.Empty;
    public string SupplierName { get => _supplierName; set => SetField(ref _supplierName, value); }

    // Commands
    public RelayCommand AddCategoryCommand { get; }
    public RelayCommand UpdateCategoryCommand { get; }
    public RelayCommand DeleteCategoryCommand { get; }

    public RelayCommand AddManufacturerCommand { get; }
    public RelayCommand UpdateManufacturerCommand { get; }
    public RelayCommand DeleteManufacturerCommand { get; }

    public RelayCommand AddSupplierCommand { get; }
    public RelayCommand UpdateSupplierCommand { get; }
    public RelayCommand DeleteSupplierCommand { get; }

    public ReferenceViewModel(ICategoryService catService, IManufacturerService manService, ISupplierService supService)
    {
        _catService = catService;
        _manService = manService;
        _supService = supService;

        Categories = _catService.GetAll();
        Manufacturers = _manService.GetAll();
        Suppliers = _supService.GetAll();

        AddCategoryCommand = new RelayCommand(() => Try(() => { _catService.Add(CategoryName); CategoryName = string.Empty; }));
        UpdateCategoryCommand = new RelayCommand(() => Try(() => { if (SelectedCategory != null) { _catService.Update(SelectedCategory.Id, CategoryName); CategoryName = string.Empty; } }), () => SelectedCategory != null);
        DeleteCategoryCommand = new RelayCommand(() => Try(() => { if (SelectedCategory != null) { _catService.Delete(SelectedCategory.Id); SelectedCategory = null; } }), () => SelectedCategory != null);

        AddManufacturerCommand = new RelayCommand(() => Try(() => { _manService.Add(ManufacturerName); ManufacturerName = string.Empty; }));
        UpdateManufacturerCommand = new RelayCommand(() => Try(() => { if (SelectedManufacturer != null) { _manService.Update(SelectedManufacturer.Id, ManufacturerName); ManufacturerName = string.Empty; } }), () => SelectedManufacturer != null);
        DeleteManufacturerCommand = new RelayCommand(() => Try(() => { if (SelectedManufacturer != null) { _manService.Delete(SelectedManufacturer.Id); SelectedManufacturer = null; } }), () => SelectedManufacturer != null);

        AddSupplierCommand = new RelayCommand(() => Try(() => { _supService.Add(SupplierName); SupplierName = string.Empty; }));
        UpdateSupplierCommand = new RelayCommand(() => Try(() => { if (SelectedSupplier != null) { _supService.Update(SelectedSupplier.Id, SupplierName); SupplierName = string.Empty; } }), () => SelectedSupplier != null);
        DeleteSupplierCommand = new RelayCommand(() => Try(() => { if (SelectedSupplier != null) { _supService.Delete(SelectedSupplier.Id); SelectedSupplier = null; } }), () => SelectedSupplier != null);
    }

    private static void Try(Action action)
    {
        try { action(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }
}
