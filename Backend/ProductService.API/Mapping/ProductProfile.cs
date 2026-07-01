using AutoMapper;
using ProductService.API.DTOs;
using ProductService.API.Models;

namespace ProductService.API.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>();

        CreateMap<CreateProductDto, Product>();

        CreateMap<UpdateProductDto, Product>();
    }
}