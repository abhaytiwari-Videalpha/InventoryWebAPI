namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Payload for POST /api/v1/auth/login.
    /// ASSUMPTION: backend expects Email + Password. Adjust here if your DTO differs.
    /// </summary>
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}