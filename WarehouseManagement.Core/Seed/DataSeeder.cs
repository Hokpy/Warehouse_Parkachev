using WarehouseData.Models;
using WarehouseManagement.Core.DataStorage;

namespace WarehouseManagement.Core.Seed;

public static class DataSeeder
{
    public static void Seed()
    {
        // Справочники
        var cat1 = new Category { Name = "Электроника" };
        var cat2 = new Category { Name = "Бытовая техника" };
        var cat3 = new Category { Name = "Инструменты" };
        InMemoryStorage.Categories.Add(cat1);
        InMemoryStorage.Categories.Add(cat2);
        InMemoryStorage.Categories.Add(cat3);

        var man1 = new Manufacturer { Name = "Samsung" };
        var man2 = new Manufacturer { Name = "Bosch" };
        var man3 = new Manufacturer { Name = "LG" };
        InMemoryStorage.Manufacturers.Add(man1);
        InMemoryStorage.Manufacturers.Add(man2);
        InMemoryStorage.Manufacturers.Add(man3);

        var sup1 = new Supplier { Name = "ООО Поставщик" };
        var sup2 = new Supplier { Name = "ИП Иванов" };
        InMemoryStorage.Suppliers.Add(sup1);
        InMemoryStorage.Suppliers.Add(sup2);

        // Организации
        var org1 = new Organization("Рога и копыта, ООО");
        var org2 = new Organization("Пупкин и сыновья, ООО");
        InMemoryStorage.Organizations.Add(org1);
        InMemoryStorage.Organizations.Add(org2);

        // Склады
        var wh1 = new Warehouse("Склад Рогов и Копыт №1", "ул. Ленина, 1", org1.OrgId);
        var wh2 = new Warehouse("Склад Рогов и Копыт №2", "ул. Ленина, 2", org1.OrgId);
        var wh3 = new Warehouse("Главный склад", "пр. Победы, 10", org2.OrgId);
        InMemoryStorage.Warehouses.Add(wh1);
        InMemoryStorage.Warehouses.Add(wh2);
        InMemoryStorage.Warehouses.Add(wh3);
        org1.Warehouses.Add(wh1);
        org1.Warehouses.Add(wh2);
        org2.Warehouses.Add(wh3);

        // Товары
        var prod1 = new Product
        {
            Article = "ART001",
            Name = "Смартфон Galaxy S24",
            Unit = "шт",
            Price = 49999.99m,
            CategoryId = cat1.Id,
            ManufacturerId = man1.Id,
            SupplierId = sup1.Id,
            DiscountPercent = 10,
            StockQuantity = 50,
            Description = "Флагманский смартфон Samsung",
            SupplierName = sup1.Name,
            Category = cat1,
            Manufacturer = man1,
            Supplier = sup1
        };

        var prod2 = new Product
        {
            Article = "ART002",
            Name = "Телевизор OLED 55\"",
            Unit = "шт",
            Price = 89999m,
            CategoryId = cat1.Id,
            ManufacturerId = man3.Id,
            SupplierId = sup1.Id,
            DiscountPercent = 5,
            StockQuantity = 15,
            Description = "OLED телевизор 55 дюймов",
            SupplierName = sup1.Name,
            Category = cat1,
            Manufacturer = man3,
            Supplier = sup1
        };

        var prod3 = new Product
        {
            Article = "ART003",
            Name = "Дрель-шуруповёрт",
            Unit = "шт",
            Price = 7500m,
            CategoryId = cat3.Id,
            ManufacturerId = man2.Id,
            SupplierId = sup2.Id,
            DiscountPercent = 0,
            StockQuantity = 100,
            Description = "Профессиональная дрель Bosch",
            SupplierName = sup2.Name,
            Category = cat3,
            Manufacturer = man2,
            Supplier = sup2
        };

        InMemoryStorage.Products.Add(prod1);
        InMemoryStorage.Products.Add(prod2);
        InMemoryStorage.Products.Add(prod3);

        wh1.Products.Add(prod1);
        wh1.Products.Add(prod2);
        wh2.Products.Add(prod3);
    }
}
