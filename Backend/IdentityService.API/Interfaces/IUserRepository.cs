using IdentityService.API.Auth.Models;

namespace IdentityService.API.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByEmailAsync(string email);

    Task<ApplicationUser?> GetByIdAsync(int id);

    Task CreateUserAsync(ApplicationUser user);

    Task UpdateUserAsync(ApplicationUser user);

    Task<ApplicationUser?> GetByRefreshTokenAsync(
    string refreshToken);
}