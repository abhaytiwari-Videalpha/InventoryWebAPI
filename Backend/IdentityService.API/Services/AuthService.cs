using AutoMapper;
using IdentityService.API.Auth.DTOs;
using IdentityService.API.Auth.Models;
using IdentityService.API.Auth.Services;
using IdentityService.API.Exceptions;
using IdentityService.API.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.API.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IMapper mapper,
        IUserRepository userRepository,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _mapper = mapper;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        var existingUser =
            await _userManager.FindByEmailAsync(dto.Email);

        if (existingUser != null)
        {
            _logger.LogWarning(
                "Registration failed. User already exists: {Email}",
                dto.Email);

            throw new BadRequestException(
                "User already exists");
        }

        var user = _mapper.Map<ApplicationUser>(dto);

        user.UserName = dto.Email;

        var result =
            await _userManager.CreateAsync(
                user,
                dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(e => e.Description));

            _logger.LogWarning(
                "Registration failed for {Email}. Errors: {Errors}",
                dto.Email,
                errors);

            throw new BadRequestException(errors);
        }

        await _userManager.AddToRoleAsync(
            user,
            "User");

        _logger.LogInformation(
            "User registered successfully: {Email}",
            dto.Email);

        return "User Registered Successfully";
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user =
            await _userManager.FindByEmailAsync(
                dto.Email);

        if (user == null)
        {
            _logger.LogWarning(
                "Login failed. User not found: {Email}",
                dto.Email);

            throw new UnauthorizedException(
                "Invalid credentials");
        }

        var validPassword =
            await _userManager.CheckPasswordAsync(
                user,
                dto.Password);

        if (!validPassword)
        {
            _logger.LogWarning(
                "Login failed. Invalid password for: {Email}",
                dto.Email);

            throw new UnauthorizedException(
                "Invalid credentials");
        }

        var token =
            await _tokenService.CreateToken(
                user,
                _userManager);

        var refreshToken =
            _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime =
            DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(user);

        _logger.LogInformation(
            "User logged in successfully: {Email}",
            dto.Email);

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(
        RefreshTokenRequestDto dto)
    {
        var user =
            await _userRepository
                .GetByRefreshTokenAsync(
                    dto.RefreshToken);

        if (user == null)
        {
            _logger.LogWarning(
                "Invalid refresh token used");

            throw new UnauthorizedException(
                "Invalid Refresh Token");
        }

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            _logger.LogWarning(
                "Expired refresh token used by user {UserId}",
                user.Id);

            throw new UnauthorizedException(
                "Refresh Token Expired");
        }

        var newJwtToken =
            await _tokenService.CreateToken(
                user,
                _userManager);

        var newRefreshToken =
            _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime =
            DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(user);

        _logger.LogInformation(
            "Token refreshed successfully for user {UserId}",
            user.Id);

        return new AuthResponseDto
        {
            Token = newJwtToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task<string> LogoutAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.MinValue;

        await _userManager.UpdateAsync(user);

        _logger.LogInformation(
            "User logged out successfully: {Email}",
            email);

        return "Logout Successful";
    }

    public async Task<ProfileResponseDto> GetProfileAsync(
    string email)
    {
        var user =
            await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            throw new NotFoundException(
                "User not found");
        }

        return new ProfileResponseDto
        {
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            ProfilePhotoUrl = user.ProfilePhotoUrl
        };
    }

    public async Task<string> UpdateProfileAsync(
    string email,
    UpdateProfileDto dto)
    {
        var user =
            await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            throw new NotFoundException(
                "User not found");
        }

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Address = dto.Address;
        user.ProfilePhotoUrl = dto.ProfilePhotoUrl;

        await _userManager.UpdateAsync(user);

        _logger.LogInformation(
            "Profile updated for {Email}",
            email);

        return "Profile Updated Successfully";
    }
}