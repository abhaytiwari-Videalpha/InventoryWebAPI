namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Payload returned inside ApiResponse&lt;AuthResponseDto&gt;.Data by the login and
    /// refresh-token endpoints. Matches IdentityService.API.Auth.DTOs.AuthResponseDto
    /// EXACTLY: property is "Token" (not "AccessToken"), plus "RefreshToken".
    /// No User object is ever returned — identity is derived client-side from JWT claims
    /// (see JwtHelper) since the backend never echoes a user/profile object here.
    /// </summary>
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;
    }
}