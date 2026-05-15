using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using WarehouseData.Models;
using WarehouseManagement.Core.Interfaces;

namespace WarehouseManagement.UI.ViewModels;

public class OrganizationViewModel : BaseViewModel
{
    private readonly IOrganizationService _orgService;
    private readonly IWarehouseService _whService;
    private readonly IProductService _prodService;
    private readonly ICategoryService _catService;
    private readonly IManufacturerService _manService;
    private readonly ISupplierService _supService;

    // --- Organizations ---
    public ObservableCollection<Organization> Organizations { get; }

    private Organization? _selectedOrganization;
    public Organization? SelectedOrganization
    {
        get => _selectedOrganization;
        set
        {
            SetField(ref _selectedOrganization, value);
            // Clear warehouse selection first (also clears products via its own setter)
            // before loading the new warehouse list for this org
            SelectedWarehouse = null;
            LoadWarehouses();
        }
    }

    private string _orgName = string.Empty;
    public string OrgName
    {
        get => _orgName;
        set => SetField(ref _orgName, value);
    }

    // --- Warehouses ---
    private ObservableCollection<Warehouse> _warehouses = new();
    public ObservableCollection<Warehouse> Warehouses
    {
        get => _warehouses;
        set => SetField(ref _warehouses, value);
    }

    private Warehouse? _selectedWarehouse;
    public Warehouse? SelectedWarehouse
    {
        get => _selectedWarehouse;
        set
        {
            SetField(ref _selectedWarehouse, value);
            LoadProducts();
        }
    }

    private string _whName = string.Empty;
    public string WhName
    {
        get => _whName;
        set => SetField(ref _whName, value);
    }

    private string _whAddress = string.Empty;
    public string WhAddress
    {
        get => _whAddress;
        set => SetField(ref _whAddress, value);
    }

    // --- Products ---
    private ObservableCollection<Product> _products = new();
    public ObservableCollection<Product> Products
    {
        get => _products;
        set => SetField(ref _products, value);
    }

    private Product? _selectedProduct;
    public Product? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            SetField(ref _selectedProduct, value);
            if (value != null) FillProductFields(value);
        }
    }

    // Product fields
    private string _productArticle = string.Empty;
    public string ProductArticle { get => _productArticle; set => SetField(ref _productArticle, value); }

    private string _productName = string.Empty;
    public string ProductName { get => _productName; set => SetField(ref _productName, value); }

    private string _productUnit = string.Empty;
    public string ProductUnit { get => _productUnit; set => SetField(ref _productUnit, value); }

    private decimal _productPrice;
    public decimal ProductPrice { get => _productPrice; set => SetField(ref _productPrice, value); }

    private int _productStock;
    public int ProductStock { get => _productStock; set => SetField(ref _productStock, value); }

    private decimal _productDiscount;
    public decimal ProductDiscount { get => _productDiscount; set => SetField(ref _productDiscount, value); }

    private string? _productDescription;
    public string? ProductDescription { get => _productDescription; set => SetField(ref _productDescription, value); }

    private string? _productPhotoPath;
    public string? ProductPhotoPath { get => _productPhotoPath; set => SetField(ref _productPhotoPath, value); }

    private int _selectedCategoryId;
    public int SelectedCategoryId { get => _selectedCategoryId; set => SetField(ref _selectedCategoryId, value); }

    private int _selectedManufacturerId;
    public int SelectedManufacturerId { get => _selectedManufacturerId; set => SetField(ref _selectedManufacturerId, value); }

    private int _selectedSupplierId;
    public int SelectedSupplierId { get => _selectedSupplierId; set => SetField(ref _selectedSupplierId, value); }

    // Filter / search
    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set { SetField(ref _searchText, value); FilterProducts(); }
    }

    private int _filterCategoryId;
    public int FilterCategoryId
    {
        get => _filterCategoryId;
        set { SetField(ref _filterCategoryId, value); FilterProducts(); }
    }

    // Lookup lists
    public ObservableCollection<Category> Categories { get; }
    public ObservableCollection<Manufacturer> Manufacturers { get; }
    public ObservableCollection<Supplier> Suppliers { get; }

    // --- Commands ---
    public RelayCommand AddOrgCommand { get; }
    public RelayCommand UpdateOrgCommand { get; }
    public RelayCommand DeleteOrgCommand { get; }
    public RelayCommand SelectOrgForEditCommand { get; }

    public RelayCommand AddWhCommand { get; }
    public RelayCommand UpdateWhCommand { get; }
    public RelayCommand DeleteWhCommand { get; }
    public RelayCommand SelectWhForEditCommand { get; }

    public RelayCommand AddProductCommand { get; }
    public RelayCommand UpdateProductCommand { get; }
    public RelayCommand DeleteProductCommand { get; }
    public RelayCommand ClearProductFormCommand { get; }
    public RelayCommand ImportCsvCommand { get; }
    public RelayCommand BrowsePhotoCommand { get; }

    private List<Product> _allProductsInWarehouse = new();

    public OrganizationViewModel(
        IOrganizationService orgService,
        IWarehouseService whService,
        IProductService prodService,
        ICategoryService catService,
        IManufacturerService manService,
        ISupplierService supService)
    {
        _orgService = orgService;
        _whService = whService;
        _prodService = prodService;
        _catService = catService;
        _manService = manService;
        _supService = supService;

        Organizations = _orgService.GetAll();
        Categories = _catService.GetAll();
        Manufacturers = _manService.GetAll();
        Suppliers = _supService.GetAll();

        AddOrgCommand = new RelayCommand(AddOrg);
        UpdateOrgCommand = new RelayCommand(UpdateOrg, () => SelectedOrganization != null);
        DeleteOrgCommand = new RelayCommand(DeleteOrg, () => SelectedOrganization != null);
        SelectOrgForEditCommand = new RelayCommand(SelectOrgForEdit);

        AddWhCommand = new RelayCommand(AddWh, () => SelectedOrganization != null);
        UpdateWhCommand = new RelayCommand(UpdateWh, () => SelectedWarehouse != null);
        DeleteWhCommand = new RelayCommand(DeleteWh, () => SelectedWarehouse != null);
        SelectWhForEditCommand = new RelayCommand(SelectWhForEdit);

        AddProductCommand = new RelayCommand(AddProduct, () => SelectedWarehouse != null);
        UpdateProductCommand = new RelayCommand(UpdateProduct, () => SelectedProduct != null);
        DeleteProductCommand = new RelayCommand(DeleteProduct, () => SelectedProduct != null);
        ClearProductFormCommand = new RelayCommand(ClearProductForm);
        ImportCsvCommand = new RelayCommand(ImportCsv, () => SelectedWarehouse != null);
        BrowsePhotoCommand = new RelayCommand(BrowsePhoto);
    }

    private void LoadWarehouses()
    {
        if (SelectedOrganization == null) { Warehouses = new(); return; }
        Warehouses = _whService.GetByOrganization(SelectedOrganization.OrgId);
    }

    private void LoadProducts()
    {
        if (SelectedWarehouse == null) { Products = new(); _allProductsInWarehouse = new(); return; }
        _allProductsInWarehouse = _prodService.GetByWarehouse(SelectedWarehouse.WhId);
        FilterProducts();
    }

    private void FilterProducts()
    {
        var filtered = _allProductsInWarehouse.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
            filtered = filtered.Where(p =>
                p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                p.Article.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        if (FilterCategoryId > 0)
            filtered = filtered.Where(p => p.CategoryId == FilterCategoryId);

        Products = new ObservableCollection<Product>(filtered);
    }

    private void FillProductFields(Product p)
    {
        ProductArticle = p.Article;
        ProductName = p.Name;
        ProductUnit = p.Unit;
        ProductPrice = p.Price;
        ProductStock = p.StockQuantity;
        ProductDiscount = p.DiscountPercent;
        ProductDescription = p.Description;
        ProductPhotoPath = p.PhotoPath;
        SelectedCategoryId = p.CategoryId;
        SelectedManufacturerId = p.ManufacturerId;
        SelectedSupplierId = p.SupplierId;
    }

    // Org commands
    private void AddOrg()
    {
        try { _orgService.Add(OrgName); OrgName = string.Empty; }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private void UpdateOrg()
    {
        if (SelectedOrganization == null) return;
        try { _orgService.Update(SelectedOrganization.OrgId, OrgName); OrgName = string.Empty; }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private void DeleteOrg()
    {
        if (SelectedOrganization == null) return;
        if (!_orgService.Delete(SelectedOrganization.OrgId))
            MessageBox.Show("Невозможно удалить организацию: у неё есть склады.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        else SelectedOrganization = null;
    }

    private void SelectOrgForEdit()
    {
        if (SelectedOrganization != null) OrgName = SelectedOrganization.OrgName;
    }

    // Warehouse commands
    private void AddWh()
    {
        if (SelectedOrganization == null) return;
        try
        {
            _whService.Add(WhName, WhAddress, SelectedOrganization.OrgId);
            // Reload snapshot so the ListBox reflects the new warehouse
            LoadWarehouses();
            WhName = string.Empty;
            WhAddress = string.Empty;
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private void UpdateWh()
    {
        if (SelectedWarehouse == null) return;
        try
        {
            _whService.Update(SelectedWarehouse.WhId, WhName, WhAddress);
            // Refresh snapshot so updated name is visible in the list
            LoadWarehouses();
            WhName = string.Empty;
            WhAddress = string.Empty;
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private void DeleteWh()
    {
        if (SelectedWarehouse == null) return;
        if (!_whService.Delete(SelectedWarehouse.WhId))
            MessageBox.Show("Невозможно удалить склад: на нём есть товары.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        else
        {
            SelectedWarehouse = null; // clears products too (via setter)
            LoadWarehouses();
        }
    }

    private void SelectWhForEdit()
    {
        if (SelectedWarehouse != null) { WhName = SelectedWarehouse.WhName; WhAddress = SelectedWarehouse.WhAddress; }
    }

    // Product commands
    private void AddProduct()
    {
        if (SelectedWarehouse == null) return;
        try
        {
            var p = BuildProductFromForm();
            _prodService.Add(p, SelectedWarehouse.WhId);
            LoadProducts();
            ClearProductForm();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private void UpdateProduct()
    {
        if (SelectedProduct == null) return;
        try
        {
            var p = BuildProductFromForm();
            _prodService.Update(p);
            LoadProducts();
            ClearProductForm();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private void DeleteProduct()
    {
        if (SelectedProduct == null) return;
        var article = SelectedProduct.Article;
        // Clear selection first so the form doesn't briefly show stale data
        SelectedProduct = null;
        ClearProductForm();
        _prodService.Delete(article);
        LoadProducts();
    }

    private void ClearProductForm()
    {
        ProductArticle = string.Empty;
        ProductName = string.Empty;
        ProductUnit = string.Empty;
        ProductPrice = 0;
        ProductStock = 0;
        ProductDiscount = 0;
        ProductDescription = null;
        ProductPhotoPath = null;
        SelectedCategoryId = 0;
        SelectedManufacturerId = 0;
        SelectedSupplierId = 0;
    }

    private Product BuildProductFromForm() => new()
    {
        Article = ProductArticle,
        Name = ProductName,
        Unit = ProductUnit,
        Price = ProductPrice,
        StockQuantity = ProductStock,
        DiscountPercent = ProductDiscount,
        Description = ProductDescription,
        PhotoPath = ProductPhotoPath,
        CategoryId = SelectedCategoryId,
        ManufacturerId = SelectedManufacturerId,
        SupplierId = SelectedSupplierId,
    };

    private void ImportCsv()
    {
        if (SelectedWarehouse == null) return;
        var dlg = new Microsoft.Win32.OpenFileDialog { Filter = "CSV files|*.csv|All files|*.*" };
        if (dlg.ShowDialog() != true) return;
        try
        {
            var csv = File.ReadAllText(dlg.FileName);
            var imported = _prodService.ImportFromCsv(csv, SelectedWarehouse.WhId);
            LoadProducts();
            MessageBox.Show($"Импортировано товаров: {imported.Count}", "Импорт CSV", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка импорта", MessageBoxButton.OK, MessageBoxImage.Error); }
    }

    private void BrowsePhoto()
    {
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All files|*.*"
        };
        if (dlg.ShowDialog() == true) ProductPhotoPath = dlg.FileName;
    }
}