using Asp.Versioning;
using IdentityService.API.Auth.DTOs;
using IdentityService.API.Interfaces;
using IdentityService.API.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("Identity Service Working");
    }
}