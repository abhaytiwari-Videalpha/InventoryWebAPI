using System.Collections.Generic;

namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Generic envelope used to deserialize standard API responses from the Gateway.
    /// Adjust property names in Module 2 if your backend's actual envelope differs.
    /// </summary>
    /// <typeparam name="T">Type of the payload returned in the "Data" field.</typeparam>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }

        public string? Message { get; set; }

        public T? Data { get; set; }

        public List<string>? Errors { get; set; }
    }
}