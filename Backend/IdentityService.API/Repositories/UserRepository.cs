using Microsoft.EntityFrameworkCore;
using IdentityService.API.Data;
using IdentityService.API.Auth.Models;
using IdentityService.API.Interfaces;

namespace IdentityService.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _context;

    public UserRepository(
        IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationUser?>
        GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                x => x.Email == email);
    }

    public async Task<ApplicationUser?>
        GetByIdAsync(int id)
    {
        return await _context.Users
            .FindAsync(id);
    }

    public async Task CreateUserAsync(
        ApplicationUser user)
    {
        _context.Users.Add(user);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(
        ApplicationUser user)
    {
        _context.Users.Update(user);

        await _context.SaveChangesAsync();
    }

    public async Task<ApplicationUser?> GetByRefreshTokenAsync(
    string refreshToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                u => u.RefreshToken == refreshToken);
    }
}