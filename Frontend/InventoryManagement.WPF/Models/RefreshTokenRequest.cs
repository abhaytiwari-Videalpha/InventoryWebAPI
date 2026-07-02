namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Payload for POST /api/v1/auth/refresh-token.
    /// ASSUMPTION: your snippet shows RefreshTokenController usage
    /// (RefreshTokenRequestDto dto) but not the DTO's own property definitions.
    /// This shape mirrors AuthResponseDto's naming convention (Token + RefreshToken),
    /// which is the most likely match given the rest of the codebase's consistency.
    /// If your actual RefreshTokenRequestDto uses different property names
    /// (e.g. just "RefreshToken" alone), tell me and this is a one-file fix.
    /// </summary>
    public class RefreshTokenRequest
    {
        public string Token { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;
    }
}