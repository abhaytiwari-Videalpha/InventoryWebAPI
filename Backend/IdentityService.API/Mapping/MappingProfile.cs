using AutoMapper;
using IdentityService.API.Auth.DTOs;
using IdentityService.API.Auth.Models;

namespace IdentityService.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterDto, ApplicationUser>();

        CreateMap<ApplicationUser, RegisterDto>();
    }
}