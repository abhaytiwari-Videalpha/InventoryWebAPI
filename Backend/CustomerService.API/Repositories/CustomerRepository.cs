using CustomerService.API.Data;
using CustomerService.API.Interfaces;
using CustomerService.API.Models;
using Microsoft.EntityFrameworkCore;
using CustomerService.API.Helpers;

namespace CustomerService.API.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly CustomerDbContext _context;

    public CustomerRepository(CustomerDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Customer> Customers, int TotalRecords)>
        GetPagedCustomersAsync(PaginationParameters parameters)
    {
        IQueryable<Customer> query =
            _context.Customers.AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            query = query.Where(c =>
                c.Name.Contains(parameters.Search) ||
                c.Email.Contains(parameters.Search) ||
                c.Phone.Contains(parameters.Search));
        }

        // Sorting
        if (!string.IsNullOrWhiteSpace(parameters.SortBy))
        {
            switch (parameters.SortBy.ToLower())
            {
                case "name":
                    query =
                        parameters.SortOrder.ToLower() == "desc"
                        ? query.OrderByDescending(c => c.Name)
                        : query.OrderBy(c => c.Name);
                    break;

                case "email":
                    query =
                        parameters.SortOrder.ToLower() == "desc"
                        ? query.OrderByDescending(c => c.Email)
                        : query.OrderBy(c => c.Email);
                    break;

                default:
                    query = query.OrderBy(c => c.CustomerId);
                    break;
            }
        }
        else
        {
            query = query.OrderBy(c => c.CustomerId);
        }

        var totalRecords = await query.CountAsync();

        var customers = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return (customers, totalRecords);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers.ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _context.Customers.FindAsync(id);
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        _context.Customers.Add(customer);

        await _context.SaveChangesAsync();

        return customer;
    }

    public async Task<Customer?> UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);

        await _context.SaveChangesAsync();

        return customer;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var customer =
            await _context.Customers.FindAsync(id);

        if (customer == null)
            return false;

        _context.Customers.Remove(customer);

        await _context.SaveChangesAsync();

        return true;
    }
}