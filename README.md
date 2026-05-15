# WarehouseManagement — Складской учёт

WPF-приложение для управления складом: организации → склады → товары.

## Структура решения

```
WarehouseManagement.sln
│
├── WarehouseData/                  # Модели данных (net8.0 library)
│   └── Models/
│       ├── Organization.cs
│       ├── Warehouse.cs
│       ├── Product.cs
│       ├── Category.cs
│       ├── Manufacturer.cs
│       └── Supplier.cs
│
├── WarehouseManagement.Core/       # Бизнес-логика (net8.0 library)
│   ├── DataStorage/InMemoryStorage.cs
│   ├── Interfaces/IServices.cs
│   ├── Services/
│   │   ├── OrganizationService.cs
│   │   ├── WarehouseService.cs
│   │   ├── ProductService.cs
│   │   └── ReferenceServices.cs
│   └── Seed/DataSeeder.cs
│
├── WarehouseManagement.Tests/      # Юнит-тесты xUnit (net8.0)
│   ├── OrganizationServiceTests.cs
│   ├── WarehouseServiceTests.cs
│   └── ProductServiceTests.cs
│
└── WarehouseManagement.UI/         # WPF-приложение (net8.0-windows)
    ├── App.xaml / App.xaml.cs
    ├── Converters/Converters.cs
    ├── Resources/Styles.xaml
    ├── ViewModels/
    │   ├── BaseViewModel.cs
    │   ├── RelayCommand.cs
    │   ├── MainViewModel.cs
    │   ├── OrganizationViewModel.cs
    │   └── ReferenceViewModel.cs
    └── Views/
        ├── MainWindow.xaml
        ├── OrganizationView.xaml
        └── ReferenceView.xaml
```

## Требования

- .NET 8 SDK
- Windows (WPF)
- Visual Studio 2022+ или Rider

## Запуск

```bash
cd WarehouseManagement
dotnet build
dotnet run --project WarehouseManagement.UI
```

## Тесты

```bash
dotnet test WarehouseManagement.Tests
```

## Функционал

- CRUD организаций, складов, товаров
- Справочники: категории, производители, поставщики
- Поиск и фильтрация товаров по тексту и категории
- Импорт товаров из CSV (разделитель `;`)
- Фото товара (выбор через диалог)
- Подсчёт цены со скидкой
- Данные-заглушки при старте (DataSeeder)
- Хранилище в памяти (InMemoryStorage)

## Формат CSV для импорта

```
Артикул;Наименование;ЕдИзм;Цена;IdКатегории;IdПроизводителя;IdПоставщика;Скидка%;Остаток;Описание;ПутьКФото
```
