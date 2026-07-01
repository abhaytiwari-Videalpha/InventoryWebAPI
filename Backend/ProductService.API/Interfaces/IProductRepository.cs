using ProductService.API.Models;

namespace ProductService.API.Interfaces;
using ProductService.API.Helpers;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync(
    PaginationParameters paginationParameters);

    Task<int> GetTotalCountAsync(PaginationParameters paginationParameters);
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
}