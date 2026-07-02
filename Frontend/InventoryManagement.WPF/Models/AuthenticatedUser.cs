using System.Collections.Generic;

namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Represents the currently authenticated user, built primarily from JWT claims.
    /// </summary>
    public class AuthenticatedUser
    {
        public string? Id { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? FullName { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}