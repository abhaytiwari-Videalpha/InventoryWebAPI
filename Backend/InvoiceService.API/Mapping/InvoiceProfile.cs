using AutoMapper;
using InvoiceService.API.DTOs;
using InvoiceService.API.Models;

namespace InvoiceService.API.Mappings;

public class InvoiceProfile : Profile
{
    public InvoiceProfile()
    {
        CreateMap<Invoice, InvoiceDto>();

        CreateMap<CreateInvoiceDto, Invoice>();

        CreateMap<UpdateInvoiceDto, Invoice>();
    }
}