using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IdentityService.API.Data;

public class IdentityDbContextFactory
    : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            "server=localhost;port=3306;database=identitydb;user=root;password=Root@123;";

        var optionsBuilder =
            new DbContextOptionsBuilder<IdentityDbContext>();

        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString));

        return new IdentityDbContext(
            optionsBuilder.Options);
    }
}