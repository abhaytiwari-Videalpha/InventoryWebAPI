using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventoryManagement.WPF.Models;
using InventoryManagement.WPF.Services;
using System.Windows;

namespace InventoryManagement.WPF.ViewModels;

public partial class AddProductViewModel : ViewModelBase
{
    private readonly ProductService _productService;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private decimal price;

    [ObservableProperty]
    private int quantity;

    public AddProductViewModel()
    {
        _productService =
            new ProductService();
    }

    [RelayCommand]
    private async Task Save()
    {
        var request =
            new CreateProductRequest
            {
                name = Name,
                Price = Price,
                Quantity = Quantity
            };

        var success =
            await _productService
                .CreateProductAsync(request);

        if (success)
        {
            MessageBox.Show(
                "Product Added Successfully");

            Name = string.Empty;
            Price = 0;
            Quantity = 0;
        }
        else
        {
            MessageBox.Show(
                "Failed To Add Product");
        }
    }
}