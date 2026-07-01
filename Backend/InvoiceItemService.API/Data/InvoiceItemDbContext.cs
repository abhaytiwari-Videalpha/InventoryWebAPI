using Microsoft.EntityFrameworkCore;
using InvoiceItemService.API.Models;

namespace InvoiceItemService.API.Data;

public class InvoiceItemDbContext : DbContext
{
    public InvoiceItemDbContext(
        DbContextOptions<InvoiceItemDbContext> options)
        : base(options)
    {
    }

    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
}