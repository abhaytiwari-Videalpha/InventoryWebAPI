using ProductService.API.Data;
using ProductService.API.Interfaces;
using ProductService.API.Models;
using Microsoft.EntityFrameworkCore;
using ProductService.API.Helpers;

namespace ProductService.API.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync(
        PaginationParameters paginationParameters)
    {
        var query = _context.Products.AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(paginationParameters.Search))
        {
            query = query.Where(p =>
                p.Name.Contains(paginationParameters.Search));
            }

        // Sorting
        if (!string.IsNullOrWhiteSpace(paginationParameters.SortBy))
        {
            switch (paginationParameters.SortBy.ToLower())
            {
                case "name":
                    query = paginationParameters.SortOrder.ToLower() == "desc"
                        ? query.OrderByDescending(p => p.Name)
                        : query.OrderBy(p => p.Name);
                    break;

                case "price":
                    query = paginationParameters.SortOrder.ToLower() == "desc"
                        ? query.OrderByDescending(p => p.Price)
                        : query.OrderBy(p => p.Price);
                    break;

                case "quantity":
                    query = paginationParameters.SortOrder.ToLower() == "desc"
                        ? query.OrderByDescending(p => p.Quantity)
                        : query.OrderBy(p => p.Quantity);
                    break;

                default:
                    query = query.OrderBy(p => p.ProductId);
                    break;
            }
        }
        else
        {
            query = query.OrderBy(p => p.ProductId);
        }

        // Filtering
        if (paginationParameters.MinPrice.HasValue)
        {
            query = query.Where(p =>
                p.Price >= paginationParameters.MinPrice.Value);
        }

        if (paginationParameters.MaxPrice.HasValue)
        {
            query = query.Where(p =>
                p.Price <= paginationParameters.MaxPrice.Value);
        }

        return await query
            .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
            .Take(paginationParameters.PageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(
    PaginationParameters paginationParameters)
{
    var query = _context.Products.AsQueryable();

    // Search
    if (!string.IsNullOrWhiteSpace(paginationParameters.Search))
    {
        query = query.Where(p =>
            p.Name.Contains(paginationParameters.Search));
    }

    // Filtering
    if (paginationParameters.MinPrice.HasValue)
    {
        query = query.Where(p =>
            p.Price >= paginationParameters.MinPrice.Value);
    }

    if (paginationParameters.MaxPrice.HasValue)
    {
        query = query.Where(p =>
            p.Price <= paginationParameters.MaxPrice.Value);
    }

    if(paginationParameters.LowStockOnly == true)
        {
            query = query.Where(p =>
            p.Quantity <= 10);
        }

    return await query.CountAsync();
}
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}