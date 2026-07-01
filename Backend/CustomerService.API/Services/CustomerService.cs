using CustomerService.API.Interfaces;
using CustomerService.API.Models;
using CustomerService.API.Helpers;

namespace CustomerService.API.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Customer>> GetAllCustomers()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Customer?> GetCustomerById(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Customer> CreateCustomer(Customer customer)
    {
        return await _repository.CreateAsync(customer);
    }

    public async Task<Customer?> UpdateCustomer(Customer customer)
    {
        return await _repository.UpdateAsync(customer);
    }

    public async Task<bool> DeleteCustomer(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<(IEnumerable<Customer> Customers, int TotalRecords)>
    GetPagedCustomers(PaginationParameters parameters)
    {
        return await _repository
            .GetPagedCustomersAsync(parameters);
    }
}