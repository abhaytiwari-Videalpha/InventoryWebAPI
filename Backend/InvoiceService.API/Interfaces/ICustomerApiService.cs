using InvoiceService.API.DTOs;

namespace InvoiceService.API.Interfaces;

public interface ICustomerApiService
{
    Task<CustomerResponseDto?> GetCustomer(int customerId);
}