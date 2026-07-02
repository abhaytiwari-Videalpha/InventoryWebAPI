using InventoryManagement.WPF.Models;

namespace InventoryManagement.WPF.Interfaces
{
    public interface IProductService
    {
        Task<PagedResponse<List<ProductDto>>> GetProductsAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string sortBy,
            string sortOrder,
            decimal? minPrice,
            decimal? maxPrice,
            bool? lowstockOnly);

        Task<ProductDto?> GetProductByIdAsync(int id);

        Task<bool> CreateProductAsync(CreateProductDto product);

        Task<bool> UpdateProductAsync(int id, UpdateProductDto product);

        Task<bool> DeleteProductAsync(int id);
    }
}