using FluentAssertions;
using Moq;
using InvoiceItemService.API.Interfaces;
using InvoiceItemService.API.Models;
using InvoiceItemService.API.Services;

namespace InvoiceItemService.Tests.Services;

public class InvoiceItemServiceTests
{
    private readonly Mock<IInvoiceItemRepository> _repoMock;
    private readonly InvoiceItemService.API.Services.InvoiceItemService _service;

    public InvoiceItemServiceTests()
    {
        _repoMock = new Mock<IInvoiceItemRepository>();

        _service =
            new InvoiceItemService.API.Services.InvoiceItemService(
                _repoMock.Object);
    }

    [Fact]
    public async Task GetInvoiceItemById_ShouldReturnItem()
    {
        var item = new InvoiceItem
        {
            InvoiceItemId = 1,
            InvoiceId = 1,
            ProductId = 1,
            Quantity = 2,
            UnitPrice = 100
        };

        _repoMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(item);

        var result =
            await _service.GetInvoiceItemById(1);

        result.Should().NotBeNull();
        result!.InvoiceItemId.Should().Be(1);
    }

    [Fact]
    public async Task GetInvoiceItemById_ShouldReturnNull_WhenNotFound()
    {
        _repoMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((InvoiceItem?)null);

        var result =
            await _service.GetInvoiceItemById(1);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllInvoiceItems_ShouldReturnItems()
    {
        var items = new List<InvoiceItem>
        {
            new()
            {
                InvoiceItemId = 1,
                ProductId = 1
            }
        };

        _repoMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(items);

        var result =
            await _service.GetAllInvoiceItems();

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateInvoiceItem_ShouldReturnCreatedItem()
    {
        var item = new InvoiceItem
        {
            InvoiceItemId = 1
        };

        _repoMock
            .Setup(x => x.CreateAsync(item))
            .ReturnsAsync(item);

        var result =
            await _service.CreateInvoiceItem(item);

        result.Should().Be(item);
    }

    [Fact]
    public async Task UpdateInvoiceItem_ShouldCallRepository()
    {
        var item = new InvoiceItem
        {
            InvoiceItemId = 1
        };

        await _service.UpdateInvoiceItem(item);

        _repoMock.Verify(
            x => x.UpdateAsync(item),
            Times.Once);
    }

    [Fact]
    public async Task DeleteInvoiceItem_ShouldCallRepository()
    {
        var item = new InvoiceItem
        {
            InvoiceItemId = 1
        };

        _repoMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(item);

        await _service.DeleteInvoiceItem(1);

        _repoMock.Verify(
            x => x.DeleteAsync(item),
            Times.Once);
    }
}