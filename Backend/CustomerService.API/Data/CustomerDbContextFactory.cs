using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CustomerService.API.Data;

public class CustomerDbContextFactory
    : IDesignTimeDbContextFactory<CustomerDbContext>
{
    public CustomerDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            "server=localhost;port=3306;database=customerdb;user=root;password=Root@123;";

        var optionsBuilder =
            new DbContextOptionsBuilder<CustomerDbContext>();

        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString));

        return new CustomerDbContext(
            optionsBuilder.Options);
    }
}