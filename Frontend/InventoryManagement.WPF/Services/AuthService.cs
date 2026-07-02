using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using InventoryManagement.WPF.Configuration;
using InventoryManagement.WPF.Helpers;
using InventoryManagement.WPF.Interfaces;
using InventoryManagement.WPF.Models;

namespace InventoryManagement.WPF.Services
{
    /// <summary>
    /// Handles login, registration, token refresh, auto-login, and logout against the
    /// Identity endpoints exposed by the Gateway. Uses a dedicated "AuthClient" HttpClient
    /// (no auth handler attached) for login/register/refresh to prevent recursive refresh
    /// loops, and the authenticated "GatewayClient" for logout, since that endpoint
    /// requires a valid Bearer token.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenStorageService _tokenStorageService;
        private readonly SemaphoreSlim _refreshLock = new(1, 1);

        private AuthenticatedUser? _currentUser;

        public AuthService(IHttpClientFactory httpClientFactory, ITokenStorageService tokenStorageService)
        {
            _httpClientFactory = httpClientFactory;
            _tokenStorageService = tokenStorageService;
        }

        public AuthenticatedUser? CurrentUser => _currentUser;

        public bool IsAuthenticated => _currentUser is not null;

        public event EventHandler<AuthenticatedUser?>? AuthenticationStateChanged;

        public async Task<ServiceResult<AuthenticatedUser>> LoginAsync(LoginRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(ApiSettings.AuthClientName);

                var httpResponse = await client.PostAsJsonAsync(ApiRoutes.Auth.Login, request, JsonOptionsProvider.Default);
                var apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>(JsonOptionsProvider.Default);

                if (!httpResponse.IsSuccessStatusCode || apiResponse?.Success != true || apiResponse.Data is null)
                {
                    return ServiceResult<AuthenticatedUser>.Fail(
                        apiResponse?.Message ?? "Invalid email or password.");
                }

                var user = ApplyAuthResult(apiResponse.Data);
                return ServiceResult<AuthenticatedUser>.Ok(user, apiResponse.Message);
            }
            catch (HttpRequestException ex)
            {
                return ServiceResult<AuthenticatedUser>.Fail($"Unable to reach the server: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ServiceResult<AuthenticatedUser>.Fail($"Unexpected error during login: {ex.Message}");
            }
        }

        public async Task<ServiceResult> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(ApiSettings.AuthClientName);

                var httpResponse = await client.PostAsJsonAsync(ApiRoutes.Auth.Register, request, JsonOptionsProvider.Default);

                // Backend returns ApiResponse<string> on register (NOT AuthResponseDto) —
                // there is no token issued at registration time.
                var apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>(JsonOptionsProvider.Default);

                if (!httpResponse.IsSuccessStatusCode || apiResponse?.Success != true)
                {
                    return ServiceResult.Fail(apiResponse?.Message ?? "Registration failed.");
                }

                return ServiceResult.Ok(apiResponse.Message ?? "Registration successful. Please log in.");
            }
            catch (HttpRequestException ex)
            {
                return ServiceResult.Fail($"Unable to reach the server: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Unexpected error during registration: {ex.Message}");
            }
        }

        public async Task<bool> RefreshTokenAsync()
        {
            await _refreshLock.WaitAsync();
            try
            {
                var storedAccessToken = _tokenStorageService.GetAccessToken();
                var storedRefreshToken = _tokenStorageService.GetRefreshToken();

                if (string.IsNullOrWhiteSpace(storedRefreshToken))
                {
                    return false;
                }

                var client = _httpClientFactory.CreateClient(ApiSettings.AuthClientName);

                var payload = new RefreshTokenRequest
                {
                    Token = storedAccessToken ?? string.Empty,
                    RefreshToken = storedRefreshToken
                };

                var httpResponse = await client.PostAsJsonAsync(ApiRoutes.Auth.RefreshToken, payload, JsonOptionsProvider.Default);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    ClearLocalSession();
                    return false;
                }

                var apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>(JsonOptionsProvider.Default);

                if (apiResponse?.Success != true || apiResponse.Data is null || string.IsNullOrWhiteSpace(apiResponse.Data.Token))
                {
                    ClearLocalSession();
                    return false;
                }

                ApplyAuthResult(apiResponse.Data);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        public async Task<ServiceResult<AuthenticatedUser>> TryAutoLoginAsync()
        {
            if (!_tokenStorageService.HasStoredSession())
            {
                return ServiceResult<AuthenticatedUser>.Fail("No stored session.");
            }

            var refreshed = await RefreshTokenAsync();

            if (!refreshed || _currentUser is null)
            {
                return ServiceResult<AuthenticatedUser>.Fail("Session expired. Please log in again.");
            }

            return ServiceResult<AuthenticatedUser>.Ok(_currentUser);
        }

        public async Task LogoutAsync()
        {
            try
            {
                // Authenticated call — GatewayClient attaches the Bearer token automatically
                // via AuthHeaderHandler. Backend identifies the user from the token's email
                // claim and invalidates their refresh token server-side.
                var client = _httpClientFactory.CreateClient(ApiSettings.GatewayClientName);
                await client.PostAsync(ApiRoutes.Auth.Logout, content: null);
            }
            catch (Exception)
            {
                // Even if the server call fails (offline, already-expired token, etc.),
                // we still clear the local session below so the user isn't stuck.
            }
            finally
            {
                ClearLocalSession();
            }
        }

        private void ClearLocalSession()
        {
            _tokenStorageService.ClearTokens();
            _currentUser = null;
            AuthenticationStateChanged?.Invoke(this, null);
        }

        /// <summary>
        /// Persists tokens, decodes JWT claims to build the current user (the backend
        /// never returns a separate user/profile object on login or refresh), and raises
        /// the authentication state changed event.
        /// </summary>
        private AuthenticatedUser ApplyAuthResult(AuthResponse authResponse)
        {
            _tokenStorageService.SaveTokens(authResponse.Token, authResponse.RefreshToken);

            var user = new AuthenticatedUser
            {
                Id = JwtHelper.GetClaimValue(authResponse.Token, "sub", "nameid", "id"),
                Email = JwtHelper.GetClaimValue(
                    authResponse.Token,
                    "email",
                    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"),
                Username = JwtHelper.GetClaimValue(authResponse.Token, "unique_name", "username", "name"),
                Roles = JwtHelper.GetRoles(authResponse.Token)
            };

            _currentUser = user;
            AuthenticationStateChanged?.Invoke(this, user);

            return user;
        }
    }
}