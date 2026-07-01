using System.Net.Http.Json;
using InventoryManagement.WPF.Models;

namespace InventoryManagement.WPF.Services;

public class AuthService : ApiService
{
    public async Task<AuthResponse?> LoginAsync(
        LoginRequest request)
    {
        var response =
            await _httpClient.PostAsJsonAsync(
                "/api/v1/Auth/login",
                request);

        if (!response.IsSuccessStatusCode)
            return null;

        var result = 
            await response.Content
                    .ReadFromJsonAsync<LoginResponse>();

        return result?.Data;
    }
}