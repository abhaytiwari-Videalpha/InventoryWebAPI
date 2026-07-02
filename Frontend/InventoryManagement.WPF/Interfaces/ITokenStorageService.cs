namespace InventoryManagement.WPF.Interfaces
{
    /// <summary>
    /// Contract for securely persisting and retrieving JWT access/refresh tokens
    /// across application sessions.
    /// </summary>
    public interface ITokenStorageService
    {
        void SaveTokens(string accessToken, string refreshToken);

        string? GetAccessToken();

        string? GetRefreshToken();

        void ClearTokens();

        bool HasStoredSession();
    }
}