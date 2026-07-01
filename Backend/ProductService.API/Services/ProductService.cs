using ProductService.API.Interfaces;
using ProductService.API.Models;
using ProductService.API.Helpers;


namespace ProductService.API.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Product>> GetAllProducts(
        PaginationParameters paginationParameters)
    {
        return await _repository.GetAllAsync(paginationParameters);
    }

    public async Task<int> GetTotalCount(
    PaginationParameters paginationParameters)
        {
            return await _repository.GetTotalCountAsync(paginationParameters);
        }

    public async Task<Product?> GetProductById(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Product> CreateProduct(Product product)
    {
        return await _repository.CreateAsync(product);
    }

    public async Task UpdateProduct(Product product)
    {
        await _repository.UpdateAsync(product);
    }

    public async Task DeleteProduct(int id)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product != null)
        {
            await _repository.DeleteAsync(product);
        }
    }
}