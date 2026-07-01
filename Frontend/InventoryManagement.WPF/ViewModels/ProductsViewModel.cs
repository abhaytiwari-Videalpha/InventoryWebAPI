using CommunityToolkit.Mvvm.ComponentModel;
using InventoryManagement.WPF.Models;
using InventoryManagement.WPF.Services;
using System.Collections.ObjectModel;

namespace InventoryManagement.WPF.ViewModels;

public partial class ProductsViewModel : ViewModelBase
{
    private readonly ProductService _productService;

    [ObservableProperty]
    private ObservableCollection<Product> products = new();

    public ProductsViewModel()
    {
        _productService =
            new ProductService();

        _ = LoadProducts();
    }

    private async Task LoadProducts()
    {
        var result =
            await _productService.GetProductsAsync();

        Products =
            new ObservableCollection<Product>(
                result);
    }
}