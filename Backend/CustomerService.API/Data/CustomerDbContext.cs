using Microsoft.EntityFrameworkCore;
using CustomerService.API.Models;

namespace CustomerService.API.Data;

public class CustomerDbContext : DbContext
{
    public CustomerDbContext(
        DbContextOptions<CustomerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
}