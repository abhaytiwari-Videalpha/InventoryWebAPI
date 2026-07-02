namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Payload for POST /api/v1/auth/register.
    /// Matches IdentityService.API.Auth.DTOs.RegisterDto EXACTLY: Email + Password only.
    /// There is no FirstName/LastName/Username at registration time in this backend —
    /// those fields belong to the Profile module (UpdateProfileDto) instead.
    /// </summary>
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}