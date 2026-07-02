using System.Windows;
using System.Windows.Controls;
using InventoryManagement.WPF.Models;
using InventoryManagement.WPF.ViewModels;

namespace InventoryManagement.WPF.Views
{
    public partial class ProductsView : UserControl
    {
        public ProductsView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Opens the row's context menu when the "⋮" button is clicked. PlacementTarget
        /// is set explicitly because opening a ContextMenu programmatically (as opposed
        /// to via right-click) does not auto-assign it.
        /// </summary>
        private void RowActionsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { ContextMenu: not null } button)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.DataContext = button.DataContext;
                button.ContextMenu.IsOpen = true;
            }
        }

        /// <summary>
        /// Invokes the EXISTING ProductsViewModel.EditProductCommand — no new
        /// ViewModel logic, just relocating the trigger from a grid button to a menu item.
        /// </summary>
        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem { DataContext: ProductDto product } &&
                DataContext is ProductsViewModel viewModel &&
                viewModel.EditProductCommand.CanExecute(product))
            {
                viewModel.EditProductCommand.Execute(product);
            }
        }

        /// <summary>
        /// Confirms with the user, then invokes the EXISTING
        /// ProductsViewModel.DeleteProductCommand. Confirmation is a View-only addition —
        /// the ViewModel's delete logic itself is untouched.
        /// </summary>
        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem { DataContext: ProductDto product } ||
                DataContext is not ProductsViewModel viewModel)
            {
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete \"{product.Name}\"? This action cannot be undone.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes && viewModel.DeleteProductCommand.CanExecute(product))
            {
                viewModel.DeleteProductCommand.Execute(product);
            }
        }

        /// <summary>
        /// Read-only product summary. View-layer only — no ViewModel/backend calls.
        /// </summary>
        private void ViewDetailsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem { DataContext: ProductDto product })
            {
                return;
            }

            var stockStatus = product.Quantity <= 0
                ? "Out of Stock"
                : product.Quantity <= Converters.ProductStatisticsConverter.LowStockThreshold
                    ? "Low Stock"
                    : "In Stock";

            var totalValue = product.Price * product.Quantity;

            MessageBox.Show(
                $"Product ID: {product.ProductId}\n" +
                $"Name: {product.Name}\n" +
                $"Price: {product.Price:N2}\n" +
                $"Quantity: {product.Quantity}\n" +
                $"Status: {stockStatus}\n" +
                $"Total Value: {totalValue:N2}",
                "Product Details",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}