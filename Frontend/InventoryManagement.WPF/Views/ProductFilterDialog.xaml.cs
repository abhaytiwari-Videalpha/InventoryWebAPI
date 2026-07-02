using System.Windows;
using InventoryManagement.WPF.Models;

namespace InventoryManagement.WPF.Views;

public partial class ProductFilterDialog : Window
{
    public ProductFilterDto Filter { get; }

    public ProductFilterDialog(ProductFilterDto filter)
    {
        InitializeComponent();

        Filter = filter;

        DataContext = Filter;
    }

    private void Apply_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void Reset_Click(
        object sender,
        RoutedEventArgs e)
    {
        Filter.MinPrice = null;
        Filter.MaxPrice = null;
        Filter.LowStockOnly = false;
        Filter.SortBy = "name";
        Filter.SortOrder = "asc";

        DataContext = null;
        DataContext = Filter;
    }
}