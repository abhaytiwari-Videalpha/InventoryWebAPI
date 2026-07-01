using InvoiceService.API.Data;
using InvoiceService.API.Helpers;
using InvoiceService.API.Interfaces;
using InvoiceService.API.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceService.API.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly InvoiceDbContext _context;

    public InvoiceRepository(InvoiceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        return await _context.Invoices.ToListAsync();
    }

    public async Task<Invoice?> GetByIdAsync(int id)
    {
        return await _context.Invoices.FindAsync(id);
    }

    public async Task<Invoice> CreateAsync(Invoice invoice)
    {
        _context.Invoices.Add(invoice);

        await _context.SaveChangesAsync();

        return invoice;
    }

    public async Task UpdateAsync(Invoice invoice)
    {
        _context.Invoices.Update(invoice);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Invoice invoice)
    {
        _context.Invoices.Remove(invoice);

        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<Invoice> Invoices, int TotalRecords)>
        GetPagedInvoicesAsync(PaginationParameters parameters)
    {
        var query = _context.Invoices.AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            query = query.Where(i =>
                i.CustomerId.ToString()
                    .Contains(parameters.Search));
        }

        // Amount Filter
        if (parameters.MinAmount.HasValue)
        {
            query = query.Where(i =>
                i.TotalAmount >= parameters.MinAmount.Value);
        }

        if (parameters.MaxAmount.HasValue)
        {
            query = query.Where(i =>
                i.TotalAmount <= parameters.MaxAmount.Value);
        }

        // Sorting
        query = parameters.SortBy?.ToLower() switch
        {
            "totalamount" =>
                parameters.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.TotalAmount)
                    : query.OrderBy(i => i.TotalAmount),

            "invoicedate" =>
                parameters.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.InvoiceDate)
                    : query.OrderBy(i => i.InvoiceDate),

            _ =>
                parameters.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(i => i.InvoiceId)
                    : query.OrderBy(i => i.InvoiceId)
        };

        var totalRecords = await query.CountAsync();

        var invoices = await query
            .Skip((parameters.PageNumber - 1)
                * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return (invoices, totalRecords);
    }
}