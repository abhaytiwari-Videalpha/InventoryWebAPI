using System.Net.Http;

namespace InventoryManagement.WPF.Services;

public class ApiService
{
    protected readonly HttpClient _httpClient;

    public ApiService()
    {
        _httpClient = new HttpClient();

        _httpClient.BaseAddress =
            new Uri("http://localhost:5078");
    }
}