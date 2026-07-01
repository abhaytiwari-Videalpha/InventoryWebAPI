using System.Windows;
using InventoryManagement.WPF.ViewModels;

namespace InventoryManagement.WPF.Views;

public partial class AddProductWindow : Window
{
    public AddProductWindow()
    {
        InitializeComponent();

        DataContext = 
            new AddProductViewModel();
    }
}