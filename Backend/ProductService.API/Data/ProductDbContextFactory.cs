using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProductService.API.Data;

public class ProductDbContextFactory
    : IDesignTimeDbContextFactory<ProductDbContext>
{
    public ProductDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<ProductDbContext>();

        optionsBuilder.UseMySql(
            "server=localhost;port=3306;database=productdb;user=root;password=Root@123;",
            ServerVersion.AutoDetect(
                "server=localhost;port=3306;database=productdb;user=root;password=Root@123;")
        );

        return new ProductDbContext(
            optionsBuilder.Options);
    }
}