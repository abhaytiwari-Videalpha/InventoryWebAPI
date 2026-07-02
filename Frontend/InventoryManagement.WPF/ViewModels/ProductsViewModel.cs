using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventoryManagement.WPF.Interfaces;
using InventoryManagement.WPF.Models;
using InventoryManagement.WPF.Views;

namespace InventoryManagement.WPF.ViewModels
{
    public partial class ProductsViewModel : ViewModelBase
    {
        private readonly IProductService _productService;

        [ObservableProperty]
        private ObservableCollection<ProductDto> products = new();

        [ObservableProperty]
        private ProductDto? selectedProduct;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private int currentPage = 1;

        [ObservableProperty]
        private int pageSize = 10;

        [ObservableProperty]
        private int totalPages;

        [ObservableProperty]
        private int totalRecords;

        [ObservableProperty]
        private string sortBy = "name";

        [ObservableProperty]
        private string sortOrder = "asc";

        [ObservableProperty]
        private decimal? minPrice;

        [ObservableProperty]
        private decimal? maxPrice;

        [ObservableProperty]
        private bool lowStockOnly;

        public ProductsViewModel(IProductService productService)
        {
            _productService = productService;

            Title = "Products";

            // Auto-load products when page opens
            _ = LoadProductsAsync();
        }

        [RelayCommand]
        private async Task LoadProductsAsync()
        {
            try
            {
                IsBusy = true;
                ClearError();

                var response = await _productService.GetProductsAsync(
                    CurrentPage,
                    PageSize,
                    SearchText,
                    SortBy,
                    SortOrder,
                    minPrice,
                    maxPrice,
                    lowStockOnly);

                Products.Clear();

                if (response.Data != null)
                {
                    foreach (var product in response.Data)
                    {
                        Products.Add(product);
                    }
                }

                TotalPages = response.TotalPages;
                TotalRecords = response.TotalRecords;
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            CurrentPage = 1;
            await LoadProductsAsync();
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            SearchText = string.Empty;
            CurrentPage = 1;

            await LoadProductsAsync();
        }

        [RelayCommand]
        private async Task NextPageAsync()
        {
            if (CurrentPage >= TotalPages)
                return;

            CurrentPage++;

            await LoadProductsAsync();
        }

        [RelayCommand]
        private async Task PreviousPageAsync()
        {
            if (CurrentPage <= 1)
                return;

            CurrentPage--;

            await LoadProductsAsync();
        }

        [RelayCommand]
        private async Task DeleteProductAsync(ProductDto? product)
        {
            if (product == null)
                return;

            try
            {
                IsBusy = true;
                ClearError();

                var success = await _productService
                    .DeleteProductAsync(product.ProductId);

                if (success)
                {
                    // Remove instantly from UI
                    Products.Remove(product);

                    TotalRecords--;

                    // Reload if page becomes empty
                    if (Products.Count == 0 && CurrentPage > 1)
                    {
                        CurrentPage--;
                        await LoadProductsAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AddProductAsync()
        {
            var dialog = new ProductDialog(
                new ProductDto());

            if (dialog.ShowDialog() != true)
                return;

            var dto = new CreateProductDto
            {
                Name = dialog.Product.Name,
                Price = dialog.Product.Price,
                Quantity = dialog.Product.Quantity
            };

            var success =
                await _productService.CreateProductAsync(dto);

            if (success)
            {
                await LoadProductsAsync();
            }
        }

        [RelayCommand]
        private async Task EditProductAsync(ProductDto? product)
        {
            if (product == null)
                return;

            var editable = new ProductDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Quantity = product.Quantity
            };

            var dialog = new ProductDialog(editable);

            if (dialog.ShowDialog() != true)
                return;

            var dto = new UpdateProductDto
            {
                ProductId = editable.ProductId,
                Name = editable.Name,
                Price = editable.Price,
                Quantity = editable.Quantity
            };

            var success =
                await _productService.UpdateProductAsync(
                    editable.ProductId,
                    dto);

            if (success)
            {
                await LoadProductsAsync();
            }
        }

        [RelayCommand]
        private async Task FilterAsync()
        {
            var dialog =
                new ProductFilterDialog(
                    new ProductFilterDto
                    {
                        MinPrice = MinPrice,
                        MaxPrice = MaxPrice,
                        LowStockOnly = LowStockOnly,
                        SortBy = SortBy,
                        SortOrder = SortOrder
                    });

            if (dialog.ShowDialog() != true)
                return;

            MinPrice = dialog.Filter.MinPrice;
            MaxPrice = dialog.Filter.MaxPrice;
            LowStockOnly = dialog.Filter.LowStockOnly;
            SortBy = dialog.Filter.SortBy;
            SortOrder = dialog.Filter.SortOrder;

            CurrentPage = 1;

            await LoadProductsAsync();
        }
    }
}