using FluentAssertions;
using InvoiceService.API.Interfaces;
using InvoiceService.API.Models;
using InvoiceService.API.Services;
using Moq;
using Xunit;

namespace InvoiceService.Tests.Services;

public class InvoiceServiceTests
{
    private readonly Mock<IInvoiceRepository> _repositoryMock;
    private readonly InvoiceService.API.Services.InvoiceService _service;

    public InvoiceServiceTests()
    {
        _repositoryMock = new Mock<IInvoiceRepository>();

        _service =
            new InvoiceService.API.Services.InvoiceService(
                _repositoryMock.Object);
    }

    [Fact]
    public async Task GetInvoiceById_ShouldReturnInvoice()
    {
        var invoice = new Invoice
        {
            InvoiceId = 1,
            CustomerId = 1,
            TotalAmount = 5000
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(invoice);

        var result =
            await _service.GetInvoiceById(1);

        result.Should().NotBeNull();
        result!.InvoiceId.Should().Be(1);
    }

    [Fact]
    public async Task GetInvoiceById_ShouldReturnNull()
    {
        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Invoice?)null);

        var result =
            await _service.GetInvoiceById(1);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllInvoices_ShouldReturnInvoices()
    {
        var invoices = new List<Invoice>
        {
            new()
            {
                InvoiceId = 1,
                CustomerId = 1,
                TotalAmount = 5000
            }
        };

        _repositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(invoices);

        var result =
            await _service.GetAllInvoices();

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateInvoice_ShouldCreateInvoice()
    {
        var invoice = new Invoice
        {
            InvoiceId = 1,
            CustomerId = 1,
            TotalAmount = 5000
        };

        _repositoryMock
            .Setup(x => x.CreateAsync(invoice))
            .ReturnsAsync(invoice);

        var result =
            await _service.CreateInvoice(invoice);

        result.Should().NotBeNull();
        result.InvoiceId.Should().Be(1);
    }

    [Fact]
    public async Task UpdateInvoice_ShouldCallRepository()
    {
        var invoice = new Invoice
        {
            InvoiceId = 1,
            CustomerId = 1,
            TotalAmount = 5000
        };

        await _service.UpdateInvoice(invoice);

        _repositoryMock.Verify(
            x => x.UpdateAsync(invoice),
            Times.Once);
    }

    [Fact]
    public async Task DeleteInvoice_ShouldDeleteInvoice()
    {
        var invoice = new Invoice
        {
            InvoiceId = 1
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(invoice);

        await _service.DeleteInvoice(1);

        _repositoryMock.Verify(
            x => x.DeleteAsync(invoice),
            Times.Once);
    }
}