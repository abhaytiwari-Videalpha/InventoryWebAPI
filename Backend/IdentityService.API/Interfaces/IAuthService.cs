using IdentityService.API.Auth.DTOs;

namespace IdentityService.API.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDto dto);

    Task<AuthResponseDto> LoginAsync(LoginDto dto);

    Task<AuthResponseDto> RefreshTokenAsync(
        RefreshTokenRequestDto dto);

    Task<string> LogoutAsync(string email);

    Task<ProfileResponseDto> GetProfileAsync(
    string email);

    Task<string> UpdateProfileAsync(
        string email,
        UpdateProfileDto dto);
}