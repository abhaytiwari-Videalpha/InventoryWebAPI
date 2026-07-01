using Asp.Versioning;
using IdentityService.API.Interfaces;
using IdentityService.API.Auth.DTOs;
using IdentityService.API.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityService.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IAuthService _authService;

    public ProfileController(
        IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var email =
            User.FindFirstValue(
                ClaimTypes.Email);

        var profile =
            await _authService
                .GetProfileAsync(email!);

        return Ok(
            new ApiResponse<ProfileResponseDto>(
                true,
                "Profile Retrieved Successfully",
                profile));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(
        UpdateProfileDto dto)
    {
        var email =
            User.FindFirstValue(
                ClaimTypes.Email);

        var result =
            await _authService
                .UpdateProfileAsync(
                    email!,
                    dto);

        return Ok(
            new ApiResponse<string>(
                true,
                result,
                result));
    }
}