using System.Windows.Controls;
using System.Windows;
using InventoryManagement.WPF.ViewModels;

namespace InventoryManagement.WPF.Views;


public partial class ProductsView : UserControl
{

    private void AddProduct_Click(
    object sender,
    RoutedEventArgs e)
    {
        var window =
            new AddProductWindow();

        window.ShowDialog();
    }
    public ProductsView()
    {
        InitializeComponent();

            DataContext =
            new ProductsViewModel();
    }

}
