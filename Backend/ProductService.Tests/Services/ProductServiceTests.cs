using Moq;
using Xunit;
using ProductService.API.Interfaces;
using ProductService.API.Models;
using ProductService.API.Services;
using ProductService.API.Helpers;

namespace ProductService.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly ProductService.API.Services.ProductService _service;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();

        _service = new ProductService.API.Services.ProductService(
            _repositoryMock.Object);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            Name = "Laptop",
            Price = 50000,
            Quantity = 10
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetProductById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result!.ProductId);
        Assert.Equal("Laptop", result.Name);
        Assert.Equal(50000, result.Price);
        Assert.Equal(10, result.Quantity);

        _repositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(100))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _service.GetProductById(100);

        // Assert
        Assert.Null(result);

        _repositoryMock.Verify(r => r.GetByIdAsync(100), Times.Once);
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnProductList()
    {
        // Arrange
        var pagination = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var products = new List<Product>
        {
            new()
            {
                ProductId = 1,
                Name = "Laptop",
                Price = 50000,
                Quantity = 10
            },
            new()
            {
                ProductId = 2,
                Name = "Mouse",
                Price = 500,
                Quantity = 25
            }
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(pagination))
            .ReturnsAsync(products);

        // Act
        var result = await _service.GetAllProducts(pagination);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());

        _repositoryMock.Verify(r => r.GetAllAsync(pagination), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreatedProduct()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            Name = "Keyboard",
            Price = 1500,
            Quantity = 15
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(product))
            .ReturnsAsync(product);

        // Act
        var result = await _service.CreateProduct(product);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Keyboard", result.Name);

        _repositoryMock.Verify(r => r.CreateAsync(product), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_ShouldCallRepository()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            Name = "Updated Laptop",
            Price = 60000,
            Quantity = 5
        };

        _repositoryMock
            .Setup(r => r.UpdateAsync(product))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateProduct(product);

        // Assert
        _repositoryMock.Verify(r => r.UpdateAsync(product), Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_ShouldDelete_WhenProductExists()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            Name = "Laptop",
            Price = 50000,
            Quantity = 10
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(product);

        _repositoryMock
            .Setup(r => r.DeleteAsync(product))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteProduct(1);

        // Assert
        _repositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(product), Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_ShouldNotDelete_WhenProductDoesNotExist()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        // Act
        await _service.DeleteProduct(999);

        // Assert
        _repositoryMock.Verify(r => r.GetByIdAsync(999), Times.Once);
        _repositoryMock.Verify(
            r => r.DeleteAsync(It.IsAny<Product>()),
            Times.Never);
    }

    [Fact]
    public async Task GetTotalCount_ShouldReturnCount()
    {
        // Arrange
        var pagination = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        _repositoryMock
            .Setup(r => r.GetTotalCountAsync(pagination))
            .ReturnsAsync(25);

        // Act
        var result = await _service.GetTotalCount(pagination);

        // Assert
        Assert.Equal(25, result);

        _repositoryMock.Verify(
            r => r.GetTotalCountAsync(pagination),
            Times.Once);
    }
}