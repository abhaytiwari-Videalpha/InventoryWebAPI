using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using InventoryManagement.WPF.Interfaces;

namespace InventoryManagement.WPF.Services
{
    /// <summary>
    /// Persists JWT access/refresh tokens to disk encrypted with Windows DPAPI
    /// (CurrentUser scope), so tokens are unreadable outside the logged-in Windows user.
    /// </summary>
    public class TokenStorageService : ITokenStorageService
    {
        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("InventoryManagement.WPF.TokenStorage");

        private readonly string _storageFilePath;

        private string? _cachedAccessToken;
        private string? _cachedRefreshToken;

        public TokenStorageService()
        {
            var appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "InventoryManagement.WPF");

            Directory.CreateDirectory(appDataFolder);

            _storageFilePath = Path.Combine(appDataFolder, "session.dat");

            LoadFromDisk();
        }

        public void SaveTokens(string accessToken, string refreshToken)
        {
            _cachedAccessToken = accessToken;
            _cachedRefreshToken = refreshToken;

            var payload = new TokenPayload
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            var json = JsonSerializer.Serialize(payload);
            var plainBytes = Encoding.UTF8.GetBytes(json);
            var encryptedBytes = ProtectedData.Protect(plainBytes, Entropy, DataProtectionScope.CurrentUser);

            File.WriteAllBytes(_storageFilePath, encryptedBytes);
        }

        public string? GetAccessToken() => _cachedAccessToken;

        public string? GetRefreshToken() => _cachedRefreshToken;

        public bool HasStoredSession() =>
            !string.IsNullOrWhiteSpace(_cachedAccessToken) && !string.IsNullOrWhiteSpace(_cachedRefreshToken);

        public void ClearTokens()
        {
            _cachedAccessToken = null;
            _cachedRefreshToken = null;

            if (File.Exists(_storageFilePath))
            {
                File.Delete(_storageFilePath);
            }
        }

        private void LoadFromDisk()
        {
            if (!File.Exists(_storageFilePath))
            {
                return;
            }

            try
            {
                var encryptedBytes = File.ReadAllBytes(_storageFilePath);
                var plainBytes = ProtectedData.Unprotect(encryptedBytes, Entropy, DataProtectionScope.CurrentUser);
                var json = Encoding.UTF8.GetString(plainBytes);

                var payload = JsonSerializer.Deserialize<TokenPayload>(json);

                if (payload is not null)
                {
                    _cachedAccessToken = payload.AccessToken;
                    _cachedRefreshToken = payload.RefreshToken;
                }
            }
            catch (CryptographicException)
            {
                // Data cannot be decrypted (different user/machine, corrupted file) — discard it.
                ClearTokens();
            }
        }

        private sealed class TokenPayload
        {
            public string? AccessToken { get; set; }
            public string? RefreshToken { get; set; }
        }
    }
}