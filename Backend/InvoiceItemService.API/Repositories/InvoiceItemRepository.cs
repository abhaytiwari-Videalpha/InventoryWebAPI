using InvoiceItemService.API.Data;
using InvoiceItemService.API.Interfaces;
using InvoiceItemService.API.Models;
using Microsoft.EntityFrameworkCore;
using InvoiceItemService.API.Helpers;

namespace InvoiceItemService.API.Repositories;

public class InvoiceItemRepository : IInvoiceItemRepository
{
    private readonly InvoiceItemDbContext _context;

    public InvoiceItemRepository(
        InvoiceItemDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InvoiceItem>> GetAllAsync()
    {
        return await _context.InvoiceItems.ToListAsync();
    }

    public async Task<InvoiceItem?> GetByIdAsync(int id)
    {
        return await _context.InvoiceItems.FindAsync(id);
    }

    public async Task<InvoiceItem> CreateAsync(
        InvoiceItem invoiceItem)
    {
        _context.InvoiceItems.Add(invoiceItem);

        await _context.SaveChangesAsync();

        return invoiceItem;
    }

    public async Task UpdateAsync(
        InvoiceItem invoiceItem)
    {
        _context.InvoiceItems.Update(invoiceItem);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(
        InvoiceItem invoiceItem)
    {
        _context.InvoiceItems.Remove(invoiceItem);

        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<InvoiceItem> InvoiceItems, int TotalRecords)>
        GetPagedInvoiceItemsAsync(
            PaginationParameters parameters)
    {
        var query =
            _context.InvoiceItems.AsQueryable();

        // Search by ProductId
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            query = query.Where(i =>
                i.ProductId.ToString()
                    .Contains(parameters.Search));
        }

        // Price Filtering
        if (parameters.MinPrice.HasValue)
        {
            query = query.Where(i =>
                i.UnitPrice >= parameters.MinPrice.Value);
        }

        if (parameters.MaxPrice.HasValue)
        {
            query = query.Where(i =>
                i.UnitPrice <= parameters.MaxPrice.Value);
        }

        // Sorting
        query =
            parameters.SortBy?.ToLower() switch
            {
                "quantity" =>
                    parameters.SortOrder == "desc"
                        ? query.OrderByDescending(i => i.Quantity)
                        : query.OrderBy(i => i.Quantity),

                "unitprice" =>
                    parameters.SortOrder == "desc"
                        ? query.OrderByDescending(i => i.UnitPrice)
                        : query.OrderBy(i => i.UnitPrice),

                "invoiceitemid" =>
                    parameters.SortOrder == "desc"
                        ? query.OrderByDescending(i => i.InvoiceItemId)
                        : query.OrderBy(i => i.InvoiceItemId),

                _ => query.OrderBy(i => i.InvoiceItemId)
            };

        var totalRecords =
            await query.CountAsync();

        var invoiceItems =
            await query
                .Skip(
                    (parameters.PageNumber - 1)
                    * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

        return (invoiceItems, totalRecords);
    }
}