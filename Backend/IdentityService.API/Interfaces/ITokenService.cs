using IdentityService.API.Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.API.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(
        ApplicationUser user,
        UserManager<ApplicationUser> userManager);

    string GenerateRefreshToken();
}