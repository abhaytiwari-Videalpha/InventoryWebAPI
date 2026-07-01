using CustomerService.API.Helpers;
using CustomerService.API.Models;

namespace CustomerService.API.Interfaces;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync();

    Task<Customer?> GetByIdAsync(int id);

    Task<Customer> CreateAsync(Customer customer);

    Task<Customer?> UpdateAsync(Customer customer);

    Task<bool> DeleteAsync(int id);

    Task<(IEnumerable<Customer> Customers, int TotalRecords)>
        GetPagedCustomersAsync(PaginationParameters parameters);
}