namespace IdentityService.API.Auth.DTOs;

public class ProfileResponseDto
{
    public string Email { get; set; } = string.Empty;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? ProfilePhotoUrl { get; set; }
}