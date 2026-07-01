namespace InventoryManagement.WPF.Helpers;

public static class TokenStorage
{
    public static string Token { get; set; } = string.Empty;

    public static string RefreshToken { get; set; } = string.Empty;
}