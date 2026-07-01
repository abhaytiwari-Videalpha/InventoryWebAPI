using AutoMapper;
using IdentityService.API.Auth.DTOs;
using IdentityService.API.Auth.Models;
using IdentityService.API.Auth.Services;
using IdentityService.API.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace IdentityService.API.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    private readonly IUserRepository _userRepository;

    private readonly IMapper _mapper;
    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IMapper mapper,
        IUserRepository userRepository)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _mapper = mapper;
        _userRepository = userRepository;
    }

    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        var existingUser =
            await _userManager.FindByEmailAsync(dto.Email);

        if (existingUser != null)
            throw new Exception("User already exists");

        var user =
            _mapper.Map<ApplicationUser>(dto);

        user.UserName = dto.Email;

        var result =
            await _userManager.CreateAsync(
                user,
                dto.Password);

        if (!result.Succeeded)
        {
            throw new Exception(
                string.Join(", ",
                    result.Errors.Select(e => e.Description)));
        }

        await _userManager.AddToRoleAsync(
            user,
            "User");

        return "User Registered Successfully";
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user =
            await _userManager.FindByEmailAsync(
                dto.Email);

        if (user == null)
            throw new Exception("User not found");

        var validPassword =
            await _userManager.CheckPasswordAsync(
                user,
                dto.Password);

        if (!validPassword)
            throw new Exception("Password incorrect");

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
            throw new Exception(
                "Invalid Refresh Token");
        }

        if (user.RefreshTokenExpiryTime
            <= DateTime.UtcNow)
        {
            throw new Exception(
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

        return new AuthResponseDto
        {
            Token = newJwtToken,
            RefreshToken = newRefreshToken
        };
    }
}