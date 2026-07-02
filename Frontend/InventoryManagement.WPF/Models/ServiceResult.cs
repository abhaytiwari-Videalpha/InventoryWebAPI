using System.Collections.Generic;

namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Normalized result returned by application services to ViewModels, decoupling
    /// ViewModels from the raw ApiResponse&lt;T&gt; wire envelope.
    /// </summary>
    public class ServiceResult
    {
        public bool Success { get; init; }

        public string? Message { get; init; }

        public List<string>? Errors { get; init; }

        public static ServiceResult Ok(string? message = null) =>
            new() { Success = true, Message = message };

        public static ServiceResult Fail(string? message, List<string>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors };
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; init; }

        public static ServiceResult<T> Ok(T data, string? message = null) =>
            new() { Success = true, Data = data, Message = message };

        public static new ServiceResult<T> Fail(string? message, List<string>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors };
    }
}