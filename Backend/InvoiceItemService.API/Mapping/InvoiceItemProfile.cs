using AutoMapper;
using InvoiceItemService.API.DTOs;
using InvoiceItemService.API.Models;

namespace InvoiceItemService.API.Mappings;

public class InvoiceItemProfile : Profile
{
    public InvoiceItemProfile()
    {
        CreateMap<InvoiceItem, InvoiceItemDto>();

        CreateMap<CreateInvoiceItemDto, InvoiceItem>();

        CreateMap<UpdateInvoiceItemDto, InvoiceItem>();
    }
}