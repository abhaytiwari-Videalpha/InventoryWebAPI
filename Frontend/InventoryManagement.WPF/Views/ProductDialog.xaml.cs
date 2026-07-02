using System.Windows;
using InventoryManagement.WPF.Models;

namespace InventoryManagement.WPF.Views
{
    public partial class ProductDialog : Window
    {
        public ProductDto Product { get; }

        public ProductDialog(ProductDto product)
        {
            InitializeComponent();

            Product = product;
            DataContext = Product;

            // Cosmetic only — does not affect Save/Cancel logic below.
            Title = product.ProductId == 0 ? "Add Product" : "Edit Product";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}