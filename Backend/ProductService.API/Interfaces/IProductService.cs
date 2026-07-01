using ProductService.API.Models;
using ProductService.API.Helpers;

namespace ProductService.API.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProducts(
    PaginationParameters paginationParameters);

    Task<int> GetTotalCount(PaginationParameters paginationParameters);
    Task<Product?> GetProductById(int id);
    Task<Product> CreateProduct(Product product);
    Task UpdateProduct(Product product);
    Task DeleteProduct(int id);
}