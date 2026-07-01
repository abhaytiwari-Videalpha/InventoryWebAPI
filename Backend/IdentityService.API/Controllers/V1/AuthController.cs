using Asp.Versioning;
using IdentityService.API.Auth.DTOs;
using IdentityService.API.Interfaces;
using IdentityService.API.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityService.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(
        IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterDto dto)
    {
        var result =
            await _authService.RegisterAsync(dto);

        return Ok(
            new ApiResponse<string>(
                true,
                "User Registered Successfully",
                result));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginDto dto)
    {
        var response =
            await _authService.LoginAsync(dto);

        return Ok(
            new ApiResponse<AuthResponseDto>(
                true,
                "Login Successful",
                response));
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        RefreshTokenRequestDto dto)
    {
        var response =
            await _authService
                .RefreshTokenAsync(dto);

        return Ok(
            new ApiResponse<AuthResponseDto>(
                true,
                "Token Refreshed Successfully",
                response));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var email =
            User.FindFirstValue(
                ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(email))
        {
            return Unauthorized(
                new ApiResponse<string>(
                    false,
                    "User information not found",
                    null));
        }

        var result =
            await _authService.LogoutAsync(email);

        return Ok(
            new ApiResponse<string>(
                true,
                "Logout Successful",
                result));
    }

    [AllowAnonymous]
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(
            new ApiResponse<string>(
                true,
                "Identity Service Working",
                "PONG"));
    }
}