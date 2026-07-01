using Moq;
using Xunit;
using CustomerService.API.Interfaces;
using CustomerService.API.Models;
using CustomerService.API.Services;

namespace CustomerService.Tests.Services;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _repositoryMock;
    private readonly CustomerService.API.Services.CustomerService _service;

    public CustomerServiceTests()
    {
        _repositoryMock =
            new Mock<ICustomerRepository>();

        _service =
            new CustomerService.API.Services.CustomerService(
                _repositoryMock.Object);
    }

    [Fact]
    public async Task GetCustomerById_ShouldReturnCustomer_WhenExists()
    {
        var customer = new Customer
        {
            CustomerId = 1,
            Name = "Abhay",
            Email = "abhay@test.com",
            Phone = "9876543210"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(customer);

        var result =
            await _service.GetCustomerById(1);

        Assert.NotNull(result);
        Assert.Equal(1, result!.CustomerId);

        _repositoryMock.Verify(
            r => r.GetByIdAsync(1),
            Times.Once);
    }

    [Fact]
    public async Task GetCustomerById_ShouldReturnNull_WhenNotFound()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(100))
            .ReturnsAsync((Customer?)null);

        var result =
            await _service.GetCustomerById(100);

        Assert.Null(result);

        _repositoryMock.Verify(
            r => r.GetByIdAsync(100),
            Times.Once);
    }

    [Fact]
    public async Task GetAllCustomers_ShouldReturnList()
    {
        var customers = new List<Customer>
        {
            new()
            {
                CustomerId = 1,
                Name = "Abhay",
                Email = "a@test.com",
                Phone = "9876543210"
            },
            new()
            {
                CustomerId = 2,
                Name = "John",
                Email = "j@test.com",
                Phone = "9999999999"
            }
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(customers);

        var result =
            await _service.GetAllCustomers();

        Assert.Equal(2, result.Count());

        _repositoryMock.Verify(
            r => r.GetAllAsync(),
            Times.Once);
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturnCustomer()
    {
        var customer = new Customer
        {
            CustomerId = 1,
            Name = "Abhay",
            Email = "abhay@test.com",
            Phone = "9876543210"
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(customer))
            .ReturnsAsync(customer);

        var result =
            await _service.CreateCustomer(customer);

        Assert.NotNull(result);

        _repositoryMock.Verify(
            r => r.CreateAsync(customer),
            Times.Once);
    }

    [Fact]
    public async Task UpdateCustomer_ShouldReturnUpdatedCustomer()
    {
        var customer = new Customer
        {
            CustomerId = 1,
            Name = "Updated",
            Email = "updated@test.com",
            Phone = "9876543210"
        };

        _repositoryMock
            .Setup(r => r.UpdateAsync(customer))
            .ReturnsAsync(customer);

        var result =
            await _service.UpdateCustomer(customer);

        Assert.NotNull(result);

        _repositoryMock.Verify(
            r => r.UpdateAsync(customer),
            Times.Once);
    }

    [Fact]
    public async Task DeleteCustomer_ShouldReturnTrue()
    {
        _repositoryMock
            .Setup(r => r.DeleteAsync(1))
            .ReturnsAsync(true);

        var result =
            await _service.DeleteCustomer(1);

        Assert.True(result);

        _repositoryMock.Verify(
            r => r.DeleteAsync(1),
            Times.Once);
    }
}