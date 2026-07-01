using System.Windows.Controls;
using System.Windows;
using InventoryManagement.WPF.Services;


namespace InventoryManagement.WPF.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
    }
    private void Products_Click(
        object sender,
        RoutedEventArgs e)
    {
        NavigationService.Instance.CurrentView =
            new ProductsView();
    }
}