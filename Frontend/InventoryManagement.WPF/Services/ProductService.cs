using System.Net.Http.Json;
using InventoryManagement.WPF.Configuration;
using InventoryManagement.WPF.Interfaces;
using InventoryManagement.WPF.Models;
using System.Net.Http;


namespace InventoryManagement.WPF.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GatewayClient");
        }

        public async Task<PagedResponse<List<ProductDto>>> GetProductsAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            string? sortBy = null,
            string? sortOrder = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? lowStockOnly = null)
        {
            var queryParams = new List<string>
            {
                $"PageNumber={pageNumber}",
                $"PageSize={pageSize}"
            };

            if (!string.IsNullOrWhiteSpace(search))
                queryParams.Add($"Search={Uri.EscapeDataString(search)}");

            if (!string.IsNullOrWhiteSpace(sortBy))
                queryParams.Add($"SortBy={sortBy}");

            if (!string.IsNullOrWhiteSpace(sortOrder))
                queryParams.Add($"SortOrder={sortOrder}");

            if (minPrice.HasValue)
                queryParams.Add($"MinPrice={minPrice}");

            if (maxPrice.HasValue)
                queryParams.Add($"MaxPrice={maxPrice}");

            if (lowStockOnly == true)
                 queryParams.Add("LowStockOnly=true");

            var endpoint =
                $"{ApiRoutes.Products}?{string.Join("&", queryParams)}";

            var response =
                await _httpClient.GetFromJsonAsync<
                    PagedResponse<List<ProductDto>>>(endpoint);

            return response ?? new PagedResponse<List<ProductDto>>
            {
                Success = false,
                Message = "Failed to retrieve products",
                Data = new List<ProductDto>()
            };
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var response =
                await _httpClient.GetFromJsonAsync<
                    ApiResponse<ProductDto>>
                    ($"{ApiRoutes.Products}/{id}");

            return response?.Data;
        }

        public async Task<bool> CreateProductAsync(CreateProductDto product)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    ApiRoutes.Products,
                    product);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateProductAsync(
            int id,
            UpdateProductDto product)
        {
            var response =
                await _httpClient.PutAsJsonAsync(
                    $"{ApiRoutes.Products}/{id}",
                    product);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var response =
                await _httpClient.DeleteAsync(
                    $"{ApiRoutes.Products}/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}