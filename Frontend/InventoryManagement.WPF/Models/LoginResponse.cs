namespace InventoryManagement.WPF.Models;

public class LoginResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public AuthResponse Data { get; set; } = new();
}