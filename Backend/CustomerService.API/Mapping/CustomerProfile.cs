using AutoMapper;
using CustomerService.API.DTOs;
using CustomerService.API.Models;

namespace CustomerService.API.Mappings;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        // Entity -> DTO
        CreateMap<Customer, CustomerDto>();

        // DTO -> Entity
        CreateMap<CreateCustomerDto, Customer>();

        // DTO -> Entity
        CreateMap<UpdateCustomerDto, Customer>();
    }
}