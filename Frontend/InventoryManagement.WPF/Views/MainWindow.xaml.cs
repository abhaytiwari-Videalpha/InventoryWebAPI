using System.Windows;
using InventoryManagement.WPF.ViewModels;

namespace InventoryManagement.WPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}