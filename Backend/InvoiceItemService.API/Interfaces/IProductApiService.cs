using InvoiceItemService.API.DTOs;

namespace InvoiceItemService.API.Interfaces;

public interface IProductApiService
{
    Task<ProductResponseDto?> GetProduct(int productId);
}