using System.Text.Json;

namespace InventoryManagement.WPF.Helpers
{
    /// <summary>
    /// Shared JsonSerializerOptions for all HTTP communication with the Gateway,
    /// tolerant of camelCase (typical ASP.NET Core default) vs PascalCase payloads.
    /// </summary>
    public static class JsonOptionsProvider
    {
        public static readonly JsonSerializerOptions Default = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}