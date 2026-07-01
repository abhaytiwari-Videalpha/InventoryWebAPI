using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InvoiceItemService.API.Data;

public class InvoiceItemDbContextFactory
    : IDesignTimeDbContextFactory<InvoiceItemDbContext>
{
    public InvoiceItemDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            "server=localhost;port=3306;database=invoiceitemdb;user=root;password=Root@123;";

        var optionsBuilder =
            new DbContextOptionsBuilder<InvoiceItemDbContext>();

        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString));

        return new InvoiceItemDbContext(
            optionsBuilder.Options);
    }
}