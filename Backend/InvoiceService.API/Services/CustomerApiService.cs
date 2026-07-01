using System.Net.Http.Json;
using InvoiceService.API.DTOs;
using InvoiceService.API.Interfaces;

namespace InvoiceService.API.Services;

public class CustomerApiService : ICustomerApiService
{
    private readonly HttpClient _httpClient;

    public CustomerApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CustomerResponseDto?> GetCustomer(
        int customerId)
    {
        return await _httpClient.GetFromJsonAsync<CustomerResponseDto>(
            $"api/v1/Customers/internal/{customerId}");
    }
}