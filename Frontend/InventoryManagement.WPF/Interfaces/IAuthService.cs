using System;
using System.Threading.Tasks;
using InventoryManagement.WPF.Models;

namespace InventoryManagement.WPF.Interfaces
{
    /// <summary>
    /// Contract for authentication operations: login, registration, silent token refresh,
    /// auto-login on startup, and logout. Also exposes the current authenticated identity.
    /// </summary>
    public interface IAuthService
    {
        AuthenticatedUser? CurrentUser { get; }

        bool IsAuthenticated { get; }

        event EventHandler<AuthenticatedUser?>? AuthenticationStateChanged;

        Task<ServiceResult<AuthenticatedUser>> LoginAsync(LoginRequest request);

        /// <summary>
        /// Registers a new account. NOTE: the backend does not return tokens on register —
        /// the user must log in separately afterward.
        /// </summary>
        Task<ServiceResult> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Attempts to refresh the access token using the stored refresh token.
        /// Used both by AuthHeaderHandler (on 401) and TryAutoLoginAsync (on startup).
        /// </summary>
        Task<bool> RefreshTokenAsync();

        Task<ServiceResult<AuthenticatedUser>> TryAutoLoginAsync();

        /// <summary>
        /// Calls the backend logout endpoint (invalidates the refresh token server-side)
        /// then clears local storage regardless of whether the server call succeeded.
        /// </summary>
        Task LogoutAsync();
    }
}