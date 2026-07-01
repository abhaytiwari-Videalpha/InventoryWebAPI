using System.Net.Http.Json;
using InvoiceItemService.API.DTOs;
using InvoiceItemService.API.Interfaces;

namespace InvoiceItemService.API.Services;

public class ProductApiService : IProductApiService
{
    private readonly HttpClient _httpClient;

    public ProductApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ProductResponseDto?> GetProduct(
        int productId)
    {
        return await _httpClient.GetFromJsonAsync<ProductResponseDto>(
            $"api/v1/Products/internal/{productId}");
    }
}