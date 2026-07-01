using InventoryManagement.WPF.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using InventoryManagement.WPF.Helpers;
using System.Net.Http;
using System.Windows;
using System.Text;


namespace InventoryManagement.WPF.Services;

public class ProductService
{
    private readonly HttpClient _httpClient;

    public ProductService()
    {
        MessageBox.Show($"TOKEN:\n{TokenStorage.Token}");

        _httpClient = new HttpClient();

        _httpClient.BaseAddress =
            new Uri("http://localhost:5078");

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                TokenStorage.Token);
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        
        var response =
            await _httpClient.GetAsync(
                "/api/v1/Products");

        response.EnsureSuccessStatusCode();

        var json =
            await response.Content.ReadAsStringAsync();

        var result =
            JsonSerializer.Deserialize<ProductResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        return result?.Data ?? new List<Product>();
    }

    public async Task<bool> CreateProductAsync(
    CreateProductRequest request)
    {
        var json =
            JsonSerializer.Serialize(request);

        var content =
            new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

        var response =
            await _httpClient.PostAsync(
                "/api/v1/Products",
                content);

        return response.IsSuccessStatusCode;
    }

}