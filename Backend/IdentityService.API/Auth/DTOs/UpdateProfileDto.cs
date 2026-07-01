namespace IdentityService.API.Auth.DTOs;

public class UpdateProfileDto
{
    public string? FirstName{get; set; }

    public string? LastName{get; set; }

    public string? Address{get; set; }

    public string? ProfilePhotoUrl{get; set; }
}