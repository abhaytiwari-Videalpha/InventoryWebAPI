using Microsoft.AspNetCore.Identity;

namespace IdentityService.API.Auth.Models;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }

    public DateTime RefreshTokenExpiryTime { get; set; }
}