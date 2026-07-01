using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InvoiceService.API.Data;

public class InvoiceDbContextFactory
    : IDesignTimeDbContextFactory<InvoiceDbContext>
{
    public InvoiceDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            "server=localhost;port=3306;database=invoicedb;user=root;password=Root@123;";

        var optionsBuilder =
            new DbContextOptionsBuilder<InvoiceDbContext>();

        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString));

        return new InvoiceDbContext(
            optionsBuilder.Options);
    }
}