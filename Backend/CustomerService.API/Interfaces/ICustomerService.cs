using CustomerService.API.Models;
using CustomerService.API.Helpers;


namespace CustomerService.API.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<Customer>> GetAllCustomers();

    Task<Customer?> GetCustomerById(int id);

    Task<Customer> CreateCustomer(Customer customer);

    Task<Customer?> UpdateCustomer(Customer customer);

    Task<bool> DeleteCustomer(int id);

    Task<(IEnumerable<Customer> Customers, int TotalRecords)>
    GetPagedCustomers(PaginationParameters parameters);
}