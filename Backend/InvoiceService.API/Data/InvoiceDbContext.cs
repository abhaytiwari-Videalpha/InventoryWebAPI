using Microsoft.EntityFrameworkCore;
using InvoiceService.API.Models;

namespace InvoiceService.API.Data;

public class InvoiceDbContext : DbContext
{
    public InvoiceDbContext(
        DbContextOptions<InvoiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Invoice> Invoices => Set<Invoice>();
}